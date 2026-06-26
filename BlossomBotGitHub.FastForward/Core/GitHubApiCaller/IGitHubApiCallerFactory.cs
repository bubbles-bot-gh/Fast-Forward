namespace BlossomBotGitHub.FastForward.Core.GitHubApiCaller;

internal interface IGitHubApiCallerFactory
{
    public IGitHubApiCaller Create();
}