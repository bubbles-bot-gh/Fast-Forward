using System.Text.Json;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward;

[UsedImplicitly]
internal class Program
{
    internal static async Task Main()
    {
        // Get webhook event info from environment
        string eventPath = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_ACTION_EVENT");
        string eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_EVENT_NAME");
        JsonDocument json = JsonDocument.Parse(await File.ReadAllTextAsync(eventPath));
        
        // Determine the event type
        (WebhookEvent webhookEvent, ActionEventType eventType) = ParseWebhookEvent(eventName, json);
        
        // Using the webhook event, set up services
        IHostApplicationBuilder builder = Host.CreateApplicationBuilder(
            new HostApplicationBuilderSettings
            {
                ContentRootPath = AppContext.BaseDirectory
            });
        builder.Services.AddAppConfig(builder.Configuration);
        builder.Services.AddAppServices(webhookEvent, eventType);
        IServiceProvider services = builder.Services.BuildServiceProvider();
        
        IActionInfo actionInfo = services.GetRequiredService<IActionInfo>();
        IGit git = services.GetRequiredService<IGit>();
        IGitHubClient gitHubClient = services.GetRequiredService<IGitHubClient>();
        ICommentBuilder commentBuilder = services.GetRequiredService<ICommentBuilder>();
        
        // Was custom command invoked?
        if (!actionInfo.EventInfo.CommandInvoked)
            Console.WriteLine("::info::Command not invoked. Skipping...");

        // Clone repo
        await git.CloneRepoAsync(actionInfo.RepoInfo.CloneUrl, "./tmp");
        
        // Create comment
        string comment = await commentBuilder.Build();
        
        // Should we exit early?
        bool shouldPostComment = actionInfo.ActionOptions.PostComment is "always" or "on-error";
        if (actionInfo.EventInfo.ShouldExit)
        {
            // There is some reason we cannot proceed. Comment output should contain reason.
            if (shouldPostComment)
                await gitHubClient.PostComment(
                    actionInfo.RepoInfo.Owner,
                    actionInfo.RepoInfo.Name,
                    (uint)actionInfo.PrInfo.IssueNumber,
                    comment);

            Environment.Exit(0);
        }
        
        // Perform fast-forward
        await gitHubClient.FastForward(
            actionInfo.RepoInfo.Owner,
            actionInfo.RepoInfo.Name,
            actionInfo.PrInfo.BaseLabel,
            actionInfo.PrInfo.HeadSha);
        
        // Post results as a comment
        await gitHubClient.PostComment(
            actionInfo.RepoInfo.Owner,
            actionInfo.RepoInfo.Name,
            (uint)actionInfo.PrInfo.IssueNumber,
            comment);
    }

    private static (WebhookEvent webhookEvent, ActionEventType eventType) ParseWebhookEvent(string eventName, 
        JsonDocument json)
    {
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
    }

    private static TWebhookEvent DeserializeWebhookEvent<TWebhookEvent>(JsonDocument json)
        where TWebhookEvent : WebhookEvent =>
            json.Deserialize<TWebhookEvent>()
            ?? throw new JsonException($"Failed to deserialize webhook event as {typeof(TWebhookEvent).Name}");
}
