using System.Diagnostics.CodeAnalysis;
using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;
using GitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.GitHubClient;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;

[UsedImplicitly]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
public sealed class GetPullRequestFixture : IAsyncLifetime
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly Mock<Octokit.IGitHubClient> MockOctokitClient;
    public readonly PullRequest SuccessExpected = new(1);
    public readonly int FailedPrNumber = 0;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubClient Subject;

    public GetPullRequestFixture()
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