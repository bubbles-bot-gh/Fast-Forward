using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientTests)]
public sealed class IsCollaboratorAsync(GitHubClientTestFixture classFixture) : IClassFixture<GitHubClientTestFixture>
{
    [Fact]
    public async Task ValidArgs_ReturnsBool()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string user = classFixture.User;
        bool expected = true;
        
        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Repository.Collaborator.IsCollaborator(owner, name, user))
            .ReturnsAsync(expected);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        bool result = await gitHubClient.IsCollaboratorAsync(owner, name, user);

        result.Should().BeTrue();
        octokitClientMock.Verify(m => m.Repository.Collaborator.IsCollaborator(owner, name, user), Times.Once);
    }

    [Fact]
    public async Task OctokitThrows_ExceptionPropagates()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        string user = classFixture.User;

        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Repository.Collaborator.IsCollaborator(owner, name, user))
            .ThrowsAsync(classFixture.OctokitException);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Func<Task<bool>> action = () => gitHubClient.IsCollaboratorAsync(owner, name, user);

        (await action.Should().ThrowAsync<NotFoundException>()).Which.Should().BeSameAs(classFixture.OctokitException);
    }
}