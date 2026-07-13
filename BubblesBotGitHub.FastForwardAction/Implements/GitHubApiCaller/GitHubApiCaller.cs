using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller.Responses;
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

    public async Task<IGhApiResponseCollaborator> GetCollaborator(string owner, string name, string user)
    {
        throw new NotImplementedException();
    }

    public async Task PostComment(string nodeId, string comment)
    {
        throw new NotImplementedException();
    }

    public async Task<IGhApiResponseCommit> GetCommit(string owner, string name, string sha)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetNodeId(string owner, string qualifiedName)
    {
        throw new NotImplementedException();
    }

    public async Task FastForward(string nodeId, string oid)
    {
        throw new NotImplementedException();
    }
}