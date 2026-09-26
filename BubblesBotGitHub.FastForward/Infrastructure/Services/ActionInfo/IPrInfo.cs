using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

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
    string? MergeBaseSha { get; }
    uint MergeBaseParentsAmount { get; }
    string PrNodeId { get; }
    string BaseNodeId { get; }
    long IssueNumber { get; }
    
    Task InitializeAsync(IGit git, IGitHubClient gitHubClient, WebhookEvent webhookEvent, ActionEventType eventType);
}