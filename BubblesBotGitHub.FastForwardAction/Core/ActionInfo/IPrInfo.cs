using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IPrInfo
{
    string BaseRef { get; }
    string BaseSha { get; }
    string HeadRef { get; }
    string HeadSha { get; }
    string HeadLabel { get; }
    string MergeBaseSha { get; }
    uint MergeBaseParentsAmount { get; }
    string PrNodeId { get; }
    string BaseNodeId { get; }
    long IssueNumber { get; }
    
    Task InitializeAsync(IGit git, IGitHubApiCaller gitHubApiCaller, WebhookEvent webhookEvent, ActionEventType eventType);
}