using System.Diagnostics.CodeAnalysis;
using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
public sealed class GetPullRequestFixture : IAsyncLifetime
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly Mock<IGitHubClient> MockOctokitClient;
    public readonly PullRequest SuccessExpected = new(1);
    public readonly int FailedPrNumber = 0;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubApiCaller Subject;

    public GetPullRequestFixture()
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