using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

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