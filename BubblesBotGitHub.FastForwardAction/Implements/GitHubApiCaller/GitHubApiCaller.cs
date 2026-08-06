using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit;

namespace BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;

internal sealed class GitHubApiCaller(IGitHubClient octokitClient) : IGitHubApiCaller
{
    public async Task<PullRequest> GetPullRequest(string owner, string name, uint prNumber)
    {
        return await octokitClient.PullRequest.Get(owner, name, (int)prNumber);
    }

    public async Task<CompareResult> GetBaseHeadComparison(string owner,
        string name,
        string baseSha,
        string headLabel)
    {
        return await octokitClient.Repository.Commit.Compare(owner, name, baseSha, headLabel);
    }

    public async Task<bool> IsCollaborator(string owner, string name, string user)
    {
        return await octokitClient.Repository.Collaborator.IsCollaborator(owner, name, user);
    }

    public async Task<IssueComment> PostComment(string owner, string name, uint issueNumber, string comment)
    {
        return await octokitClient.Issue.Comment.Create(owner, name, issueNumber, comment);
    }

    public async Task<GitHubCommit> GetCommit(string owner, string name, string sha)
    {
        return await octokitClient.Repository.Commit.Get(owner, name, sha);
    }

    public async Task<Reference> FastForward(string owner, string name, string baseLabel, string headSha)
    {
        ReferenceUpdate refUpdate = new(headSha, force: false);
        
        return await octokitClient.Git.Reference.Update(owner, name, baseLabel, refUpdate);
    }
}