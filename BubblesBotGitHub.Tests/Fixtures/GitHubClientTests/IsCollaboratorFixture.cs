using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;
using GitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.GitHubClient;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;

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