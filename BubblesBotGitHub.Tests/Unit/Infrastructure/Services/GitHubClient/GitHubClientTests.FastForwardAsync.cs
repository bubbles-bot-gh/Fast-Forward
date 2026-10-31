using System.Linq.Expressions;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientTests)]
public sealed class FastForwardAsync(GitHubClientTestFixture classFixture) : IClassFixture<GitHubClientTestFixture>
{
    [Fact]
    public async Task ValidArgs_ReturnsReference()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string baseLabel = $"{owner}/{name}";
        string headSha = classFixture.HeadSha;
        Reference expected = new();
        Expression<Func<ReferenceUpdate, bool>> matchesUpdate = r => r.Sha == headSha && r.Force == false;

        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Git.Reference.Update(owner, name,baseLabel, It.Is(matchesUpdate)))
            .ReturnsAsync(expected);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Reference result = await gitHubClient.FastForwardAsync(owner, name, baseLabel, headSha);

        result.Should().BeSameAs(expected);
        octokitClientMock.Verify(m => m.Git.Reference.Update(owner, name, baseLabel, It.Is(matchesUpdate)), Times.Once);
    }

    [Fact]
    public async Task OctokitThrows_ExceptionPropagates()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string baseLabel = $"{owner}/{name}";
        string headSha = classFixture.HeadSha;
        Expression<Func<ReferenceUpdate, bool>> matchesUpdate = r => r.Sha == headSha && r.Force == false;
        
        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Git.Reference.Update(owner, name, baseLabel, It.Is(matchesUpdate)))
            .ThrowsAsync(classFixture.OctokitException);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Func<Task<Reference>> action = () => gitHubClient.FastForwardAsync(owner, name, baseLabel, headSha);

        (await action.Should().ThrowAsync<NotFoundException>()).Which.Should().BeSameAs(classFixture.OctokitException);
    }
}