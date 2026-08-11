using BubblesBotGitHub.FastForward.Core.ActionInfo;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal sealed class ActionInfoFactory(
    IActionOptions actionOptions,
    IRepoInfo repoInfo,
    IEventInfo eventInfo) : IActionInfoFactory
{
    public IActionInfo Create(WebhookEvent webhookEvent)
    {
        return new ActionInfo(actionOptions, repoInfo, eventInfo);
    }
}