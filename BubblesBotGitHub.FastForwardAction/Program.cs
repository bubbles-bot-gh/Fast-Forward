using System.Text.Json;
using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward;

[UsedImplicitly]
internal class Program
{
    private static async Task Main(string[] args)
    {
        // Get webhook event info from environment
        string eventPath = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_ACTION_EVENT");
        string eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_EVENT_NAME");
        string json = await File.ReadAllTextAsync(eventPath);
        
        (WebhookEvent webhookEvent, ActionEventType eventType) = ParseWebhookEvent(eventName, json);
        
        // Build service provider
        IServiceCollection services = new ServiceCollection();
        IServiceProvider serviceProvider = services
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        // Get services
        IActionInfo actionInfo = serviceProvider.GetRequiredService<IActionInfo>();
        IGitHubApiCaller githubApiCaller = serviceProvider.GetRequiredService<IGitHubApiCaller>();
        IGit git = serviceProvider.GetRequiredService<IGit>();
        
        // Was custom command invoked?
        if (!actionInfo.EventInfo.CommandInvoked)
            Console.WriteLine("::info::Command not invoked. Skipping...");
        
        // TODO: Implement auto merge on pull request opened/edited
        
        // Clone repo
        await git.CloneRepoAsync(actionInfo.RepoInfo.CloneUrl, "./tmp");
        
        // Create comment
        
        // Post comment
        
        // Perform fast-forward
        
        /* TODO:
            1) Was custom command invoked?
            2) Clone repo
            3) Create comment
            4) Post comment
            5) Perform fast-forward
        */
        
        Console.WriteLine("Hello, World!");
    }

    private static (WebhookEvent webhookEvent, ActionEventType eventType) ParseWebhookEvent(string eventName, 
        string json)
    {
        return eventName switch
        {
            "pull_request_opened" => (DeserializeWebhookEvent<PullRequestOpenedEvent>(json), 
                ActionEventType.PullRequestOpened),
            "issue_comment_edited" => (DeserializeWebhookEvent<IssueCommentEditedEvent>(json), 
                ActionEventType.IssueCommentEdited),
            "issue_comment_created" => (DeserializeWebhookEvent<IssueCommentCreatedEvent>(json),
                ActionEventType.IssueCommentCreated),
            _ => throw new NotSupportedException($"Unsupported event type: {eventName}")
        };
    }

    private static TWebhookEvent DeserializeWebhookEvent<TWebhookEvent>(string json)
        where TWebhookEvent : WebhookEvent =>
            JsonSerializer.Deserialize<TWebhookEvent>(json)
            ?? throw new JsonException($"Failed to deserialize webhook event as {typeof(TWebhookEvent).Name}");
}