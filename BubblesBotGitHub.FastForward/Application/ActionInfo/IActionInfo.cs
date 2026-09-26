using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

namespace BubblesBotGitHub.FastForward.Application.Interfaces;

public interface IActionInfo
{
    IPrInfo PrInfo { get; }
    IRepoInfo RepoInfo { get; }
    IActionOptions ActionOptions { get; }
    IEventInfo EventInfo { get; }
}