using System.Runtime.CompilerServices;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using Microsoft.Extensions.Options;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal sealed record PrInfo(IOptions<Config> Opts) : IPrInfo
{
    private IGit _git = null!;
    private IGitHubClient _gitHubClient = null!;
    private WebhookEvent _webhookEvent = null!;

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

    // TODO: Ensure this gets called in service container extensions!
    public async Task InitializeAsync(IGit git,
        IGitHubClient gitHubClient,
        WebhookEvent webhookEvent,
        ActionEventType eventType)
    {
        _git = git;
        _gitHubClient = gitHubClient;
        _webhookEvent = webhookEvent;
        
        Dictionary<ActionEventType, Func<WebhookEvent, Task>> extractionMap = new() 
        {
            { ActionEventType.PullRequestOpened, ParsePullRequestOpenedEvent },
            { ActionEventType.IssueCommentCreated , ParseIssueCommentEvent<IssueCommentCreatedEvent> },
            { ActionEventType.IssueCommentEdited, ParseIssueCommentEvent<IssueCommentEditedEvent> }
        };
        
        await extractionMap[eventType].Invoke(_webhookEvent);
    }

    private async Task ParsePullRequestOpenedEvent(WebhookEvent webhookEvent)
    {
        PullRequestOpenedEvent eventData = webhookEvent as PullRequestOpenedEvent
            ?? throw new InvalidOperationException($"Could not cast '{typeof(WebhookEvent).FullName}' to '{typeof(PullRequestOpenedEvent).FullName}'.'");

        HeadRepoName = eventData.PullRequest.Head.Repo.Name;
        HeadRepoOwner = eventData.PullRequest.Head.User.Login;
        BaseRef = eventData.PullRequest.Base.Ref;
        BaseSha = eventData.PullRequest.Base.Sha;
        BaseLabel = eventData.PullRequest.Base.Label;
        HeadRef = eventData.PullRequest.Head.Ref;
        HeadSha = eventData.PullRequest.Head.Sha;
        HeadLabel = eventData.PullRequest.Head.Label;
        MergeBaseSha = await _git.GetMergeBaseSha(BaseSha, HeadSha, Opts.Value.WorkingDir);
        MergeBaseParentsAmount = await _git.GetAmountOfParents(MergeBaseSha, Opts.Value.WorkingDir);
        PrNodeId = eventData.PullRequest.NodeId;
        IssueNumber = eventData.PullRequest.Number;
        BaseNodeId = eventData.PullRequest.Base.Repo.NodeId;
    }

    private async Task ParseIssueCommentEvent<TWebhookEvent>(WebhookEvent webhookEvent) where TWebhookEvent : class
    {
        TWebhookEvent eventData = webhookEvent as TWebhookEvent
            ?? throw new InvalidOperationException($"Cannot cast '{webhookEvent.GetType()}' to '{typeof(TWebhookEvent).FullName}'.");
        
        // Verify the repository exists
        if (webhookEvent.Repository is null)
        {
            await PrintRepositoryNotFound();
            
            return;
        }
        
        string repoOwner = webhookEvent.Repository.Owner.Login;
        string repoName = webhookEvent.Repository.Name;
        IssueNumber = (webhookEvent as IssueCommentEvent)?.Issue.Number 
            ?? throw new InvalidOperationException($"Could not cast {webhookEvent.GetType()} to '{typeof(TWebhookEvent).FullName}'.");
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
        MergeBaseSha = await _git.GetMergeBaseSha(BaseSha, HeadSha, Opts.Value.WorkingDir);
        MergeBaseParentsAmount = await _git.GetAmountOfParents(MergeBaseSha, Opts.Value.WorkingDir);
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
            Console.WriteLine($"::warning "
                + $"file={([CallerFilePath] string path = "") => path},"
                + $"endLine={([CallerLineNumber] int num = 0) => num}"
                + $"::{msg}");

            return;
        }
            
        await File.AppendAllTextAsync(summaryPath, msg);
    }
}