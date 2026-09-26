using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Moq;
using Octokit;
using GitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.GitHubClient;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;

[UsedImplicitly]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
public sealed class FastForwardFixture : IAsyncLifetime
{
    public readonly Mock<Octokit.IGitHubClient> MockOctokitClient;
    public readonly IGitHubClient Subject;
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string HeadSha = "abc123";
    public readonly string BaseLabel = "heads/some-branch";
    public readonly Reference ExpectedSuccess = new();

    public FastForwardFixture()
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