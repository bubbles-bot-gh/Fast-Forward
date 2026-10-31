using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientTests)]
public sealed class GetCommitAsync(GitHubClientTestFixture classFixture) : IClassFixture<GitHubClientTestFixture>
{
    [Fact]
    public async Task ValidArgs_ReturnsGitHubCommit()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string sha = classFixture.BaseSha;
        GitHubCommit expected = new();

        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Repository.Commit.Get(owner, name, sha))
            .ReturnsAsync(expected);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        GitHubCommit result = await gitHubClient.GetCommitAsync(owner, name, sha);

        result.Should().BeSameAs(expected);
        octokitClientMock.Verify(m => m.Repository.Commit.Get(owner, name, sha), Times.Once);
    }

    [Fact]
    public async Task OctokitThrows_ExceptionPropagates()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string sha = classFixture.BaseSha;

        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Repository.Commit.Get(owner, name, sha))
            .ThrowsAsync(classFixture.OctokitException);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Func<Task<GitHubCommit>> action = () => gitHubClient.GetCommitAsync(owner, name, sha);

        (await action.Should().ThrowAsync<NotFoundException>()).Which.Should().BeSameAs(classFixture.OctokitException);
    }
}