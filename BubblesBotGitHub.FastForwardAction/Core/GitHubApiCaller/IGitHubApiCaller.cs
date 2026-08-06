using Octokit;

namespace BubblesBotGitHub.FastForward.Core.GitHubApiCaller;

internal interface IGitHubApiCaller
{
    public Task<PullRequest> GetPullRequest(string owner, string name, uint prNumber);
    public Task<CompareResult> GetBaseHeadComparison(string owner,
        string name,
        string baseSha,
        string headLabel);
    public Task<bool> IsCollaborator(string owner, string name, string user);
    public Task<IssueComment> PostComment(string owner, string name, uint issueNumber, string comment);
    public Task<GitHubCommit> GetCommit(string owner, string name, string sha);
    public Task<Reference> FastForward(string owner, string name, string baseLabel, string headSha);
}