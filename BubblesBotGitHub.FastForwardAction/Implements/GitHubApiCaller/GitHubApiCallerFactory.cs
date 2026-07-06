using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;

namespace BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;

internal class GitHubApiCallerFactory : IGitHubApiCallerFactory
{
    public IGitHubApiCaller Create()
    {
        return new GitHubApiCaller();
    }

    private string GetInstallationToken(string installationId)
    {
        return "";
    }
}