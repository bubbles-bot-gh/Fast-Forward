using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class GetBaseHeadComparisonFixture
{
    public const string Owner = "bubbles-bot-gh";
    public const string Name = "fast-forward";
    public const string BaseSha = "abc123";
    public const string HeadLabel = "tests/some-head-label";
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    public readonly CompareResult SuccessExpected= new(
        url: "",
        htmlUrl: "",
        permalinkUrl: "",
        diffUrl: "",
        patchUrl: "",
        new GitHubCommit(),
        new GitHubCommit(),
        status: "ahead",
        aheadBy: 0,
        behindBy: 0,
        totalCommits: 0,
        commits: [],
        files: []);

    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
}