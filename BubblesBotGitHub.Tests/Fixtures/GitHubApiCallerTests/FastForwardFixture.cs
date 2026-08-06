using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class FastForwardFixture
{
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    public const string Owner = "bubbles-bot-gh";
    public const string Name = "fast-forward";
    public const string HeadSha = "abc123";
    public const string BaseLabel = "heads/some-branch";
    public readonly Reference ExpectedSuccess = new();
}