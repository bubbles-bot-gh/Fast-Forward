using BlossomBotGitHub.FastForward.Core.GitHubApiCaller;

namespace BlossomBotGitHub.FastForward.Implements.GitHubApiCaller;

internal class GitHubApiCallerFactory : IGitHubApiCallerFactory
{
    public IGitHubApiCaller Create()
    {
        return new GitHubApiCaller();
    }

    private string GetInstallationToken(string installationId)
    {
        
    }
}