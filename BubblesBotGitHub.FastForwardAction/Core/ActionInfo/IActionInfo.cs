namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IActionInfo
{
    IApiCaller ApiCaller { get; }
    IRepoInfo RepoInfo { get; }
    IActionOptions ActionOptions { get; }
    IEventInfo EventInfo { get; }
}