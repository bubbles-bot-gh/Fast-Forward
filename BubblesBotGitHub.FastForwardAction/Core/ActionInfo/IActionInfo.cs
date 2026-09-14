namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

public interface IActionInfo
{
    IPrInfo PrInfo { get; }
    IRepoInfo RepoInfo { get; }
    IActionOptions ActionOptions { get; }
    IEventInfo EventInfo { get; }
}