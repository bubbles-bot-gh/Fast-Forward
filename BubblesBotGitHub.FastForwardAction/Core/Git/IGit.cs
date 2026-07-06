namespace BubblesBotGitHub.FastForward.Core.Git;

internal interface IGit
{
    public Task CloneRepoAsync(string cloneUrl, string workingDir);
    public Task<string> Log(string exclude, string baseSha, string headSha, string workingDir);
    public Task<string> GetMergeBaseSha(string baseSha, string headSha, string workingDir);
    public Task<uint> GetAmountOfParents(string sha, string workingDir);
}