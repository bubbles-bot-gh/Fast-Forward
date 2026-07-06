using BubblesBotGitHub.FastForward.Core.GraphQL;

namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IApiCaller
{
    Task<IPrResponse> GetPullRequest(string owner, string repoName, int prNumber);
    Task<IComparisonResponse> GetBaseHeadComparison(string owner, string repoName, string baseSha, string headLabel);
    Task<ICollaboratorResponse> GetCollaborators(string owner, string repoName, string user);
    Task PostComment(string nodeId, string comment);
    Task<ICommit> GetCommit(string owner, string repoName, string sha);
    Task<string> GetNodeId(string owner, string repoName, string qualifiedName);
    Task FastForward(string nodeId, string oid);
}