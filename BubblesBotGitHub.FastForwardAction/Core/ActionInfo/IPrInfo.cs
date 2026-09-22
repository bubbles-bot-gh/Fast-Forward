using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit;
using Octokit.Webhooks;
using IGitHubClient = BubblesBotGitHub.FastForward.Core.GitHubApiCaller.IGitHubClient;

namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

public interface IPrInfo
{
    string BaseRef { get; }
    string BaseSha { get; }
    string BaseLabel { get; }
    string HeadRef { get; }
    string HeadSha { get; }
    string HeadLabel { get; }
    string HeadRepoOwner { get; }
    string HeadRepoName { get; }
    string MergeBaseSha { get; }
    uint MergeBaseParentsAmount { get; }
    string PrNodeId { get; }
    string BaseNodeId { get; }
    long IssueNumber { get; }
    
    Task InitializeAsync(IGit git, IGitHubClient gitHubClient, WebhookEvent webhookEvent, ActionEventType eventType);
}