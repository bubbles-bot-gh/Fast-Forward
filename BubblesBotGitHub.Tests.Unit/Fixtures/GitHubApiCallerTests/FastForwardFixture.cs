using System.Diagnostics.CodeAnalysis;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
public sealed class FastForwardFixture : IAsyncLifetime
{
    public readonly Mock<IGitHubClient> MockOctokitClient;
    public readonly IGitHubApiCaller Subject;
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string HeadSha = "abc123";
    public readonly string BaseLabel = "heads/some-branch";
    public readonly Reference ExpectedSuccess = new();

    public FastForwardFixture()
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