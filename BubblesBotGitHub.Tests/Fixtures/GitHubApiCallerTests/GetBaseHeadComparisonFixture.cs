using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

public class GetBaseHeadComparisonFixture
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string BaseSha = "abc123";
    public readonly string HeadLabel = "tests/some-head-label";
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    internal IGitHubApiCaller GetSubject(IGitHubClient client) => new GitHubApiCaller(client);
    public CompareResult Expected= new(
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