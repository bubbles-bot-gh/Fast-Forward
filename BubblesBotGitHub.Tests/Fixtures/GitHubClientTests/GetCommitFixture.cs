using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;
using GitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.GitHubClient;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;

[UsedImplicitly]
public sealed class GetCommitFixture : IAsyncLifetime
{
    public readonly Mock<Octokit.IGitHubClient> MockOctokitClient;
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubClient Subject;
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
        MockOctokitClient = new Mock<Octokit.IGitHubClient>(MockBehavior.Strict);
        Subject = new GitHubClient(MockOctokitClient.Object);
    }
    
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        MockOctokitClient.Reset();
        
        return ValueTask.CompletedTask;
    }
}