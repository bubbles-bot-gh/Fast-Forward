using System.Net;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class IsCollaboratorFixture
{
    public const string Owner = "bubbles-bot-gh";
    public const string Name = "fast-forward";
    public const string User = "luneisolei";
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    public const bool SuccessExpected = true;
    public readonly NotFoundException NotFoundException = new("Not Found", HttpStatusCode.NotFound);
}