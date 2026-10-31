using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using Microsoft.Extensions.Options;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal sealed class PrInfo(IOptions<Config> opts, WebhookEvent webhookEvent, ActionEventType eventType) : IPrInfo
{
    private IGit _git = null!;
    private IGitHubClient _gitHubClient = null!;
    
    public string HeadRepoOwner { get; private set; } = string.Empty;
    public string HeadRepoName { get; private set; } = string.Empty;
    public string BaseRef { get; private set; } = string.Empty;
    public string BaseSha { get; private set; } = string.Empty;
    public string BaseLabel { get; private set; } = string.Empty;
    public string HeadRef { get; private set; } = string.Empty;
    public string HeadSha { get; private set; } = string.Empty;
    public string HeadLabel { get; private set; } = string.Empty;
    public string MergeBaseSha { get; private set; } = string.Empty;
    public uint MergeBaseParentsAmount { get; private set; }
    public string PrNodeId { get; private set; } = string.Empty;
    public string BaseNodeId { get; private set; } = string.Empty;
    public long IssueNumber { get; private set; }
    
    public async Task InitializeAsync(IGit git, IGitHubClient gitHubClient)
    {
        _git = git;
        _gitHubClient = gitHubClient;
        
        // Verify the repository exists
        if (webhookEvent.Repository is null)
        {
            await PrintRepositoryNotFound();
            
            return;
        }
        
        Dictionary<ActionEventType, Func<WebhookEvent, Task>> extractionMap = new() 
        {
            { ActionEventType.PullRequestOpened, ParsePullRequestOpenedEvent },
            { ActionEventType.IssueCommentCreated , ParseIssueCommentEvent<IssueCommentCreatedEvent> },
            { ActionEventType.IssueCommentEdited, ParseIssueCommentEvent<IssueCommentEditedEvent> }
        };
        
        await extractionMap[eventType].Invoke(webhookEvent);
    }

    private async Task ParsePullRequestOpenedEvent(WebhookEvent webhookEvent)
    {
        PullRequestOpenedEvent eventData = webhookEvent as PullRequestOpenedEvent
            ?? throw new InvalidOperationException($"Could not cast '{typeof(WebhookEvent).FullName}' to "
                + $"'{typeof(PullRequestOpenedEvent).FullName}'.'");

        HeadRepoName = eventData.PullRequest.Head.Repo.Name;
        HeadRepoOwner = eventData.PullRequest.Head.User.Login;
        BaseRef = eventData.PullRequest.Base.Ref;
        BaseSha = eventData.PullRequest.Base.Sha;
        BaseLabel = eventData.PullRequest.Base.Label;
        HeadRef = eventData.PullRequest.Head.Ref;
        HeadSha = eventData.PullRequest.Head.Sha;
        HeadLabel = eventData.PullRequest.Head.Label;
        MergeBaseSha = await _git.GetMergeBaseSha(BaseSha, HeadSha, opts.Value.WorkingDir);
        MergeBaseParentsAmount = await _git.GetParentCount(MergeBaseSha, opts.Value.WorkingDir);
        PrNodeId = eventData.PullRequest.NodeId;
        IssueNumber = eventData.PullRequest.Number;
        BaseNodeId = eventData.PullRequest.Base.Repo.NodeId;
    }

    private async Task ParseIssueCommentEvent<TWebhookEvent>(WebhookEvent webhookEvent) where TWebhookEvent : WebhookEvent
    {
        TWebhookEvent eventData = webhookEvent as TWebhookEvent
            ?? throw new InvalidOperationException($"Could not cast '{typeof(WebhookEvent).FullName}' to '{typeof(TWebhookEvent).FullName}'.");
        
        string repoOwner = eventData.Repository.Owner.Login;
        string repoName = eventData.Repository.Name;
        IssueNumber = (eventData as IssueCommentEvent)!.Issue.Number; // Suppressed because it's impossible for webhookEvent to be null here.
        Octokit.PullRequest pr = await _gitHubClient.GetPullRequestAsync(repoOwner, repoName, IssueNumber);

        await ParseIssueCommentPrInfo(pr);
    }

    private async Task ParseIssueCommentPrInfo(Octokit.PullRequest pr)
    {
        HeadRepoOwner = pr.Head.User.Login;
        HeadRepoName = pr.Head.Repository.Name;
        BaseSha = pr.Base.Sha;
        BaseRef = pr.Base.Ref;
        BaseLabel = pr.Base.Label;
        HeadRef = pr.Head.Ref;
        HeadSha = pr.Head.Sha;
        HeadLabel = pr.Head.Label;
        MergeBaseSha = await _git.GetMergeBaseSha(BaseSha, HeadSha, opts.Value.WorkingDir);
        MergeBaseParentsAmount = await _git.GetParentCount(MergeBaseSha, opts.Value.WorkingDir);
        PrNodeId = pr.NodeId;
        BaseNodeId = pr.Base.NodeId;
    }

    private async Task PrintRepositoryNotFound()
    {
        string? summaryPath = Environment.GetEnvironmentVariable("GITHUB_SUMMARY_PATH");
        const string msg = "Repository is null. The repository has either been deleted or otherwise made inaccessible.";
            
        if (summaryPath is null)
        {
            Console.WriteLine("::warning::GITHUB_SUMMARY_PATH environment variable not set.");
            Console.WriteLine($"::warning::{msg}");

            return;
        }
            
        await File.AppendAllTextAsync(summaryPath, msg);
    }
}