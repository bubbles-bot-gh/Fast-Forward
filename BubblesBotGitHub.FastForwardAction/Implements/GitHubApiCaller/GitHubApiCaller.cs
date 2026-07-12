using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller.Responses;
using Octokit;

namespace BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;

internal sealed class GitHubApiCaller(IGitHubClient octokitClient) : IGitHubApiCaller
{
    public async Task<IGhApiResponsePr> GetPullRequest(string repoOwner, string repoName, uint prNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<IGhApiResponseCompare> GetBaseHeadComparison(string repoOwner, string repoName, string baseSha, string headLabel)
    {
        throw new NotImplementedException();
    }

    public async Task<IGhApiResponseCollaborator> GetCollaborator(string repoOwner, string repoName, string user)
    {
        throw new NotImplementedException();
    }

    public async Task PostComment(string nodeId, string comment)
    {
        throw new NotImplementedException();
    }

    public async Task<IGhApiResponseCommit> GetCommit(string repoOwner, string repoName, string sha)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetNodeId(string repoOwner, string qualifiedName)
    {
        throw new NotImplementedException();
    }

    public async Task FastForward(string nodeId, string oid)
    {
        throw new NotImplementedException();
    }
}