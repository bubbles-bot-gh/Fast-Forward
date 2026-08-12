using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class GetPullRequestFixture
{
    public const string Owner = "bubbles-bot-gh";
    public const string Name = "fast-forward";
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    public readonly PullRequest SuccessExpected = new(1);
    public const uint FailedPrNumber = 0;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
}