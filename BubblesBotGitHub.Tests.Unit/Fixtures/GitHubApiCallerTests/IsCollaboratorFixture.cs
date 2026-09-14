using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class IsCollaboratorFixture : IAsyncLifetime
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly string User = "luneisolei";
    public readonly Mock<IGitHubClient> MockOctokitClient;
    public readonly bool SuccessExpected = true;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
    public readonly IGitHubApiCaller Subject;

    public IsCollaboratorFixture()
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