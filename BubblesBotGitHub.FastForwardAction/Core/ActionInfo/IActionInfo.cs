namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IActionInfo
{
    IRepoInfo RepoInfo { get; }
    IActionOptions ActionOptions { get; }
    IEventInfo EventInfo { get; }
}