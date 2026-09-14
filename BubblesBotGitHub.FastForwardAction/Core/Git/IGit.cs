namespace BubblesBotGitHub.FastForward.Core.Git;

public interface IGit
{
    public Task CloneRepoAsync(string cloneUrl, string workingDir);
    public Task<string> LogCommitGraph(string exclude, string baseSha, string headSha, string workingDir);
    public Task<string> GetMergeBaseSha(string baseSha, string headSha, string workingDir = "/tmp");
    public Task<uint> GetAmountOfParents(string sha, string workingDir = "/tmp");
}