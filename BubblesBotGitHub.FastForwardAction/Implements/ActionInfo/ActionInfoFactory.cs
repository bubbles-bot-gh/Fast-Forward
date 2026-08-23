using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal sealed class ActionInfoFactory(
    IActionOptions actionOptions,
    IRepoInfo repoInfo,
    IEventInfo eventInfo,
    IPrInfo prInfo,
    IGit git,
    IGitHubApiCaller ghCaller,
    WebhookEvent webhookEvent,
    ActionEventType eventType) : IActionInfoFactory
{
    public IActionInfo Create()
    {
        ActionInfo actionInfo = new(actionOptions, repoInfo, eventInfo, prInfo);
        actionInfo.PrInfo.InitializeAsync(git, ghCaller, webhookEvent, eventType);
        
        return actionInfo;
    }
}