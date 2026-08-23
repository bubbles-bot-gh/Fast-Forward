using BubblesBotGitHub.FastForward.Core.ActionInfo;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal sealed record ActionInfo(
    IActionOptions ActionOptions,
    IRepoInfo RepoInfo,
    IEventInfo EventInfo,
    IPrInfo PrInfo) : IActionInfo
{
    public IPrInfo PrInfo { get; } = PrInfo;
    public IRepoInfo RepoInfo { get; } = RepoInfo;
    public IActionOptions ActionOptions { get; } = ActionOptions;
    public IEventInfo EventInfo { get; } = EventInfo;
}