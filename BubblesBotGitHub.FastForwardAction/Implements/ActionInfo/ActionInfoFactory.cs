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
    IGitHubClient ghCaller,
    WebhookEvent webhookEvent,
    ActionEventType eventType) : IActionInfoFactory
{
    public async Task<IActionInfo> Create()
    {
        ActionInfo actionInfo = new(actionOptions, repoInfo, eventInfo, prInfo);
        await actionInfo.PrInfo.InitializeAsync(git, ghCaller, webhookEvent, eventType);
        
        return actionInfo;
    }
}