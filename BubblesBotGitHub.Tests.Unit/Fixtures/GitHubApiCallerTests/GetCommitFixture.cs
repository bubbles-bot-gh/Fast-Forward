using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class GetCommitFixture : IAsyncLifetime
{
    public readonly Mock<IGitHubClient> MockOctokitClient;
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubApiCaller Subject;
    public readonly GitHubCommit SuccessExpected = new(
        nodeId: "",
        url: "",
        label: "",
        @ref: "",
        sha: "abc123",
        user: new User(),
        repository: new Repository(),
        author: new Author(),
        commentsUrl: "",
        commit: new Commit(),
        committer: new Author(),
        htmlUrl: "",
        stats: new GitHubCommitStats(),
        parents: [],
        files: []
    );

    public GetCommitFixture()
    {
        MockOctokitClient = new Mock<IGitHubClient>(MockBehavior.Strict);
        Subject = new GitHubApiCaller(MockOctokitClient.Object);
    }
    
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        MockOctokitClient.Reset();
        
        return ValueTask.CompletedTask;
    }
}