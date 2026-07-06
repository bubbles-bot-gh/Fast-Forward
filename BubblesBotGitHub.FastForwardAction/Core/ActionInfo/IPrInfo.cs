using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IPrInfo
{
    Task FinishInitialization(IApiCaller apiCaller, WebhookEvent webhookEvent, ActionEventType eventType);
    string BaseRef { get; }
    string BaseSha { get; }
    string HeadRef { get; }
    string HeadSha { get; }
    string HeadLabel { get; }
    string HeadOwner { get; }
    string HeadRepo { get; }
    string MergeBaseSha { get; }
    string MergeBaseParentsAmount { get; }
    string PrNodeId { get; }
    string BaseNodeId { get; }
    string HeadNodeId { get; }
    uint IssueNumber { get; }
    void SetEvent(WebhookEvent webhookEvent, ActionEventType eventType);
}