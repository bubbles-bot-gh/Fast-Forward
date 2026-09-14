using System.Runtime.CompilerServices;
using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal sealed record PrInfo : IPrInfo
{
    private IGit _git = null!;
    private IGitHubApiCaller _gitHubApiCaller = null!;
    private WebhookEvent _webhookEvent = null!;
    
    public string BaseRef { get; private set; } = string.Empty;
    public string BaseSha { get; private set; } = string.Empty;
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
        IGitHubApiCaller gitHubApiCaller,
        WebhookEvent webhookEvent,
        ActionEventType eventType)
    {
        _git = git;
        _gitHubApiCaller = gitHubApiCaller;
        _webhookEvent = webhookEvent;
        
        Dictionary<ActionEventType, Func<Task>> extractionMap = new() 
        {
            { ActionEventType.PullRequestOpened, ExtractDataFromPullRequestOpenedEvent },
            { ActionEventType.IssueCommentCreated , ExtractDataFromIssueCommentEvent },
            { ActionEventType.IssueCommentEdited, ExtractDataFromIssueCommentEvent }
        };
        
        await extractionMap[eventType].Invoke();
    }

    private async Task ExtractDataFromPullRequestOpenedEvent()
    {
        PullRequestOpenedEvent eventData = (PullRequestOpenedEvent)_webhookEvent;
        
        BaseRef = eventData.PullRequest.Base.Ref;
        BaseSha = eventData.PullRequest.Base.Sha;
        HeadRef = eventData.PullRequest.Head.Ref;
        HeadSha = eventData.PullRequest.Head.Sha;
        HeadLabel = eventData.PullRequest.Head.Label;
        MergeBaseSha = await _git.GetMergeBaseSha(BaseSha, HeadSha);
        MergeBaseParentsAmount = await _git.GetAmountOfParents(MergeBaseSha);
        PrNodeId = eventData.PullRequest.NodeId;
        IssueNumber = eventData.PullRequest.Number;
        BaseNodeId = eventData.PullRequest.Base.Repo.NodeId;
    }

    private async Task ExtractDataFromIssueCommentEvent()
    {
        IssueCommentCreatedEvent eventData = (IssueCommentCreatedEvent)_webhookEvent;
        
        // Verify the repository exists
        if (eventData.Repository is null)
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

            return;
        }
        
        string repoOwner = eventData.Repository.Owner.Login;
        string repoName = eventData.Repository.Name;
        IssueNumber = eventData.Issue.Number;
        Octokit.PullRequest pr = await _gitHubApiCaller.GetPullRequestAsync(repoOwner, repoName, IssueNumber);
        
        BaseSha = pr.Base.Sha;
        BaseRef = pr.Base.Ref;
        HeadRef = pr.Head.Ref;
        HeadSha = pr.Head.Sha;
        HeadLabel = pr.Head.Label;
        MergeBaseSha = await _git.GetMergeBaseSha(BaseSha, HeadSha);
        MergeBaseParentsAmount = await _git.GetAmountOfParents(BaseSha);
        PrNodeId = pr.NodeId;
        BaseNodeId = pr.Base.NodeId;
    }
}