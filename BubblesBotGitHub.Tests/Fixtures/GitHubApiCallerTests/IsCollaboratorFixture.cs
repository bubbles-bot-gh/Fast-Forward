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
public sealed class IsCollaboratorFixture : IAsyncLifetime
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string User = "luneisolei";
    public readonly Mock<Octokit.IGitHubClient> MockOctokitClient;
    public readonly bool SuccessExpected = true;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubClient Subject;

    public IsCollaboratorFixture()
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