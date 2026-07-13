using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public class GetPullRequestFixture
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "fast-forward";
    public readonly uint PrNumber = 1;
    public readonly Mock<IGitHubClient> MockOctokitClient = new(MockBehavior.Strict);
    internal IGitHubApiCaller GetSubject(IGitHubClient client) => new GitHubApiCaller(client);
}