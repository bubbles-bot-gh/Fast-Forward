namespace BlossomBotGitHub.FastForward.Core.ActionInfo;

internal interface IActionInfo
{
    IApiCaller ApiCaller { get; }
    IRepoInfo RepoInfo { get; }
    IOptions Options { get; }
    IEventInfo EventInfo { get; }
}