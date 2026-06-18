namespace BlossomBotGitHub.FastForward.Core.Git;

internal interface IGit
{
    Task CloneRepoAsync(string cloneUrl, string workingDir);
}