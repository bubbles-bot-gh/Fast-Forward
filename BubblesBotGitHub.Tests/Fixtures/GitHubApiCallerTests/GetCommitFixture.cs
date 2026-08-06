using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class GetCommitFixture
{
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    public const string Owner = "bubbles-bot-gh";
    public const string Name = "fast-forward";
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
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
}