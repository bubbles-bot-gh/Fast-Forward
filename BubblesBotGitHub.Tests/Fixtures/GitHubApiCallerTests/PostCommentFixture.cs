using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class PostCommentFixture
{
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    public const string Owner = "bubbles-bot-gh";
    public const string Name = "fast-forward";
    public const uint IssueNumber = 1;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
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
}