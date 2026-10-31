using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward;

[UsedImplicitly]
internal class Program
{
    [ExcludeFromCodeCoverage]
    internal static async Task Main()
    {
       Environment.Exit(await FastForwardRunner());
    }

    internal static async Task<int> FastForwardRunner(IHost? host = null)
    {
        (WebhookEvent webhookEvent, ActionEventType eventType) = await ParseWebhookEvent();
        host ??= BuildHost(webhookEvent, eventType);
        
        // Using scope, get registered services that do not require a cloned repo
        using IServiceScope scope = host.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        IRepoInfo repoInfo = services.GetRequiredService<IRepoInfo>();
        IGit git = services.GetRequiredService<IGit>();
        IOptions<Config> opts = services.GetRequiredService<IOptions<Config>>();
        
        // Clone our repo, so we can read git information from it
        await git.CloneRepoAsync(repoInfo.CloneUrl, opts.Value.WorkingDir);
        
        // Get the rest of our registered services which do require a clone repo
        IActionInfo actionInfo = services.GetRequiredService<IActionInfo>();
        IGitHubClient gitHubClient = services.GetRequiredService<IGitHubClient>();
        ICommentBuilder commentBuilder = services.GetRequiredService<ICommentBuilder>();
        
        // Was custom command invoked?
        if (!actionInfo.EventInfo.CommandInvoked)
        {
            Console.WriteLine("::info::Command not invoked. Skipping...");
            
            return 0;
        }
        
        // Build comment
        string comment = await commentBuilder.Build();
        
        // Only post a comment if postComment is "always" or "on-error"
        string postComment = actionInfo.ActionOptions.PostComment;
        if (actionInfo.EventInfo.ShouldExit)
        {
            // There is some reason we cannot proceed. Comment output should contain reason.
            if (postComment is "always" or "on-error")
                await gitHubClient.PostCommentAsync(
                    actionInfo.RepoInfo.Owner,
                    actionInfo.RepoInfo.Name,
                    (uint)actionInfo.PrInfo.IssueNumber,
                    comment);

            return 1;
        }
        
        // Check auto merge if we're in a pull request, otherwise fast-forward
        if (actionInfo.ActionOptions.IsAutoMerge || eventType != ActionEventType.PullRequestOpened)
        {
            // Perform fast-forward
            await gitHubClient.FastForwardAsync(
                actionInfo.RepoInfo.Owner,
                actionInfo.RepoInfo.Name,
                actionInfo.PrInfo.BaseLabel,
                actionInfo.PrInfo.HeadSha);
        }
        
        // Check if we can post a comment
        if (postComment is "never" or "on-error") return 0;
        
        // Post results as a comment
        await gitHubClient.PostCommentAsync(
            actionInfo.RepoInfo.Owner,
            actionInfo.RepoInfo.Name,
            (uint)actionInfo.PrInfo.IssueNumber,
            comment);

        return 0;
    }
    
    internal static async Task<(WebhookEvent, ActionEventType)> ParseWebhookEvent()
    {
        string eventPath = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_ACTION_EVENT");
        string eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_EVENT_NAME");
        JsonDocument json = JsonDocument.Parse(await File.ReadAllTextAsync(eventPath));
        
        string? action = null;
        if(json.RootElement.TryGetProperty("action", out JsonElement a))
            action = a.GetString();

        string? specificEvent = action is null ? eventName : $"{eventName}_{action}";
        
        return specificEvent switch
        {
            "pull_request_opened" => (DeserializeWebhookEvent<PullRequestOpenedEvent>(json), 
                ActionEventType.PullRequestOpened),
            "issue_comment_edited" => (DeserializeWebhookEvent<IssueCommentEditedEvent>(json), 
                ActionEventType.IssueCommentEdited),
            "issue_comment_created" => (DeserializeWebhookEvent<IssueCommentCreatedEvent>(json),
                ActionEventType.IssueCommentCreated),
            _ => throw new NotSupportedException($"Unsupported event type: {specificEvent}")
        };
        
        TWebhookEvent DeserializeWebhookEvent<TWebhookEvent>(JsonDocument json)
            where TWebhookEvent : WebhookEvent =>
            json.Deserialize<TWebhookEvent>()
            ?? throw new JsonException($"Failed to deserialize webhook event as {typeof(TWebhookEvent).Name}");
    }

    internal static IHost BuildHost(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        // Using the webhook event, set up services
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(
            new HostApplicationBuilderSettings
            {
                ContentRootPath = AppContext.BaseDirectory
            });
        builder.Services.AddAppConfig(builder.Configuration);
        builder.Services.AddAppServices(webhookEvent, eventType);
        IHost host = builder.Build();
        
        // Access options to trigger validation
        _ = host.Services.GetRequiredService<IOptions<Config>>().Value;
        
        return host;
    }
}
