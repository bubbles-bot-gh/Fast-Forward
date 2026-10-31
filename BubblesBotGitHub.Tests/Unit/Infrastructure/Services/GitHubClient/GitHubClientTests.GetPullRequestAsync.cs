using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Octokit;
using PullRequest = Octokit.PullRequest;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientTests)]
public sealed class GetPullRequestAsync(GitHubClientTestFixture classFixture) : IClassFixture<GitHubClientTestFixture>
{
    [Fact]
    public async Task ValidArgs_ReturnPullRequest()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        int pullRequestNumber = classFixture.PullRequestNumber;
        
        PullRequest expected = new PullRequest();
        Mock<Octokit.IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.PullRequest.Get(owner, name, pullRequestNumber)).ReturnsAsync(expected);
        
        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        PullRequest pullRequest = await gitHubClient.GetPullRequestAsync(classFixture.Owner, classFixture.Name, classFixture.PullRequestNumber);

        pullRequest.Should().BeSameAs(expected);
        octokitClientMock.Verify(m => m.PullRequest.Get(owner, name, pullRequestNumber), Times.Once);
    }

    [Fact]
    public async Task OctokitThrows_ExceptionPropagates()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        int pullRequestNumber = classFixture.PullRequestNumber;
        
        Mock<Octokit.IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.PullRequest.Get(owner, name, pullRequestNumber)).ThrowsAsync(classFixture.OctokitException);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Func<Task<PullRequest>> action = () => gitHubClient.GetPullRequestAsync(owner, name, pullRequestNumber);

        (await action.Should().ThrowAsync<NotFoundException>()).Which.Should().BeSameAs(classFixture.OctokitException);
    }
}