namespace BubblesBotGitHub.FastForward.Core.GitHubApiCaller;

internal interface IGitHubApiCallerFactory
{
    public IGitHubApiCaller Create();
}