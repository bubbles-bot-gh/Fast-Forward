using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class GetBaseHeadComparisonFixture : IAsyncLifetime
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string BaseSha = "abc123";
    public readonly string HeadLabel = "tests/some-head-label";
    public readonly Mock<IGitHubClient> MockOctokitClient;
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
    public readonly IGitHubApiCaller Subject;
    
    public GetBaseHeadComparisonFixture()
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