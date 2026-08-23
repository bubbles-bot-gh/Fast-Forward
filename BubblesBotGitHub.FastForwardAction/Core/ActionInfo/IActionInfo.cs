namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IActionInfo
{
    IPrInfo PrInfo { get; }
    IRepoInfo RepoInfo { get; }
    IActionOptions ActionOptions { get; }
    IEventInfo EventInfo { get; }
}