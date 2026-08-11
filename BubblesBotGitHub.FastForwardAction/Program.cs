using System.Text.Json;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Implements;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward;

class Program
{
    static void Main(string[] args)
    {
        // Get JSON representing the webhook event
        string eventPath = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_ACTION_EVENT");
        string eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME")
            ?? throw new InvalidOperationException("Missing environment variable GITHUB_EVENT_NAME");
        string json = File.ReadAllText(eventPath);

        // Deserialize JSON as WebhookEvent
        WebhookEvent gitHubEvent = eventName switch
        {
            "pull_request_opened" => JsonSerializer.Deserialize<PullRequestOpenedEvent>(json) ?? throw new JsonException(),
            "issue_comment_created" => JsonSerializer.Deserialize<IssueCommentCreatedEvent>(json) ?? throw new JsonException(),
            _ => throw new NotSupportedException($"Unsupported event type: {eventName}")
        };
        
        // Build service provider
        IServiceCollection services = new ServiceCollection();
        IServiceProvider serviceProvider = services
            .AddAppServices(gitHubEvent)
            .BuildServiceProvider();
        
        // Create IActionInfo
        IActionInfo actionInfo = serviceProvider.GetRequiredService<IActionInfo>();
        
        Console.WriteLine("Hello, World!");
    }
}