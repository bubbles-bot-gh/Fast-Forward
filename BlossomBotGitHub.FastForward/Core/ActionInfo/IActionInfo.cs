namespace BlossomBotGitHub.FastForward.Core;

public interface IActionInfo
{
    IApiCaller ApiCaller { get; }
    IRepoInfo RepoInfo { get; }
    IOptions Options { get; }
    IEventInfo EventInfo { get; }
}