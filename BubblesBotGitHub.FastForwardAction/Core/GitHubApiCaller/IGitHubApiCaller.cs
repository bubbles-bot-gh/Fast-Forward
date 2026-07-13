using BubblesBotGitHub.FastForward.Core.GitHubApiCaller.Responses;
using Octokit;

namespace BubblesBotGitHub.FastForward.Core.GitHubApiCaller;

internal interface IGitHubApiCaller
{
    public Task<PullRequest> GetPullRequest(string owner, string name, uint prNumber);

    public Task<CompareResult> GetBaseHeadComparison(string owner,
        string name,
        string baseSha,
        string headLabel);

    public Task<IGhApiResponseCollaborator> GetCollaborator(string owner, string name, string user);

    public Task PostComment(string nodeId, string comment);
    
    public Task<IGhApiResponseCommit> GetCommit(string owner, string name, string sha);
    public Task<string> GetNodeId(string owner, string qualifiedName);
    public Task FastForward(string nodeId, string oid);
}