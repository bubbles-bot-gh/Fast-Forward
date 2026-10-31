using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientTests)]
public sealed class GetBaseHeadComparisonAsync(GitHubClientTestFixture classFixture) : IClassFixture<GitHubClientTestFixture>
{
    [Fact]
    public async Task ValidArgs_ReturnsCompareResult()
    {
        CompareResult expected = new CompareResult();
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string baseSha = classFixture.BaseSha;
        string headLabel = $"{owner}/{name}";
        
        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Repository.Commit.Compare(owner, name, baseSha, headLabel))
            .ReturnsAsync(expected);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        CompareResult result = await gitHubClient.GetBaseHeadComparison(owner, name, baseSha, headLabel);

        result.Should().BeSameAs(expected);
        octokitClientMock.Verify(m => m.Repository.Commit.Compare(owner, name, baseSha, headLabel), Times.Once);
    }

    [Fact]
    public async Task OctokitThrows_ExceptionPropagates()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string baseSha = classFixture.BaseSha;
        string headLabel = $"{owner}/{name}";
        
        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Repository.Commit.Compare(owner, name, baseSha, headLabel))
            .ThrowsAsync(classFixture.OctokitException);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Func<Task<CompareResult>> action = () => gitHubClient.GetBaseHeadComparison(owner, name, baseSha, headLabel);

        (await action.Should().ThrowAsync<NotFoundException>()).Which.Should().BeSameAs(classFixture.OctokitException);
    }
}