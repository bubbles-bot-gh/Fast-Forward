using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Git;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal sealed class ActionInfoFactory(
    IActionOptions actionOptions,
    IRepoInfo repoInfo,
    IEventInfo eventInfo,
    IPrInfo prInfo,
    IGit git,
    IGitHubClient gitHubClient) : IActionInfoFactory
{
    public async Task<IActionInfo> Create()
    {
        ActionInfo actionInfo = new(actionOptions, repoInfo, eventInfo, prInfo);
        await actionInfo.PrInfo.InitializeAsync(git, gitHubClient);
        
        return actionInfo;
    }
}