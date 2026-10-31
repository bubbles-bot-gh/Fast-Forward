using Octokit;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services;

public interface IGitHubClient
{
    public Task<PullRequest> GetPullRequestAsync(string owner, string name, long prNumber);
    public Task<CompareResult> GetBaseHeadComparison(string owner,
        string name,
        string baseSha,
        string headLabel);
    public Task<bool> IsCollaboratorAsync(string owner, string name, string user);
    public Task<IssueComment> PostCommentAsync(string owner, string name, uint issueNumber, string comment);
    public Task<GitHubCommit> GetCommitAsync(string owner, string name, string sha);
    public Task<Reference> FastForwardAsync(string owner, string name, string baseLabel, string headSha);
}