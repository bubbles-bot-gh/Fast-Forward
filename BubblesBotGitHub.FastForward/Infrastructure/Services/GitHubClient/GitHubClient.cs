using Octokit;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services;

public sealed class GitHubClient(Octokit.IGitHubClient octokitClient) : IGitHubClient
{
    public async Task<PullRequest> GetPullRequestAsync(string owner, string name, long prNumber)
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

    public async Task<bool> IsCollaboratorAsync(string owner, string name, string user)
    {
        bool response = await octokitClient.Repository.Collaborator.IsCollaborator(owner, name, user);
        Console.WriteLine($"::info::{user} {(response ? "is" : "is not")} a collaborator for '{owner}/{name}'");
        
        return response;
    }

    public async Task<IssueComment> PostCommentAsync(string owner, string name, uint issueNumber, string comment)
    {
        return await octokitClient.Issue.Comment.Create(owner, name, issueNumber, comment);
    }

    public async Task<GitHubCommit> GetCommitAsync(string owner, string name, string sha)
    {
        return await octokitClient.Repository.Commit.Get(owner, name, sha);
    }

    public async Task<Reference> FastForwardAsync(string owner, string name, string baseRef, string headSha)
    {
        ReferenceUpdate refUpdate = new(headSha, force: false);
        Console.WriteLine($"::notice::Fast-forwarding '{baseRef}' to '{headSha}' on repository '{owner}/{name}'.");
        return await octokitClient.Git.Reference.Update(owner, name, baseRef, refUpdate);
    }
}