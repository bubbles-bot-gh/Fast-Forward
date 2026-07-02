using BlossomBotGitHub.FastForward.Core.GitHubApiCaller.Responses;

namespace BlossomBotGitHub.FastForward.Core.GitHubApiCaller;

internal interface IGitHubApiCaller
{
    public Task<IGhApiResponsePr> GetPullRequest(string repoOwner, string repoName, uint prNumber);

    public Task<IGhApiResponseCompare> GetBaseHeadComparison(string repoOwner,
        string repoName,
        string baseSha,
        string headLabel);

    public Task<IGhApiResponseCollaborator> GetCollaborator(string repoOwner, string repoName, string user);

    public Task PostComment(string nodeId, string comment);
    
    public Task<IGhApiResponseCommit> GetCommit(string repoOwner, string repoName, string sha);
    public Task<string> GetNodeId(string repoOwner, string qualifiedName);
    public Task FastForward(string nodeId, string oid);
}