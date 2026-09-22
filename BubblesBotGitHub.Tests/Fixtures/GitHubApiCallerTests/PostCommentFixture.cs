using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;
using GitHubClient = BubblesBotGitHub.FastForward.Implements.GitHubApiCaller.GitHubClient;
using IGitHubClient = BubblesBotGitHub.FastForward.Core.GitHubApiCaller.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class PostCommentFixture : IAsyncLifetime
{
    public readonly Mock<Octokit.IGitHubClient> MockOctokitClient;
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly uint IssueNumber = 1;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubClient Subject;
    public readonly IssueComment SuccessExpected = new(
        id: 1,
        nodeId: "",
        url: "",
        htmlUrl: "",
        body: "This is a test comment!",
        createdAt: DateTime.Now,
        updatedAt: DateTime.Now,
        user: new User(),
        reactions: new ReactionSummary(),
        authorAssociation: new AuthorAssociation());
    
    public readonly IssueComment FailureExpected = new(
        id: 1,
        nodeId: "",
        url: "",
        htmlUrl: "",
        body: null,
        createdAt: DateTime.Now,
        updatedAt: DateTime.Now,
        user: new User(),
        reactions: new ReactionSummary(),
        authorAssociation: new AuthorAssociation());

    public PostCommentFixture()
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