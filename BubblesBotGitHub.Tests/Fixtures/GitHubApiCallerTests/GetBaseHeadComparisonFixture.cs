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
public sealed class GetBaseHeadComparisonFixture : IAsyncLifetime
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string BaseSha = "abc123";
    public readonly string HeadLabel = "tests/some-head-label";
    public readonly Mock<Octokit.IGitHubClient> MockOctokitClient;
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
    public readonly IGitHubClient Subject;
    
    public GetBaseHeadComparisonFixture()
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