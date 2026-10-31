using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientTests)]
public sealed class PostCommentAsync(GitHubClientTestFixture classFixture) : IClassFixture<GitHubClientTestFixture>
{
    [Fact]
    public async Task ValidArgs_ReturnsIssueComment()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        uint issueNumber = classFixture.IssueNumber;
        string comment = classFixture.Comment;
        IssueComment expected = new();
        
        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Issue.Comment.Create(owner, name, issueNumber, comment))
            .ReturnsAsync(expected);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        IssueComment result = await gitHubClient.PostCommentAsync(owner, name, issueNumber, comment);

        result.Should().BeSameAs(expected);
        octokitClientMock.Verify(m => m.Issue.Comment.Create(owner, name, issueNumber, comment), Times.Once);
    }

    [Fact]
    public async Task OctokitThrows_ExceptionPropagates()
    {
        string owner = classFixture.Owner;
        string name = classFixture.Name;
        uint issueNumber = classFixture.IssueNumber;
        string comment = classFixture.Comment;

        Mock<IGitHubClient> octokitClientMock = new(MockBehavior.Strict);
        octokitClientMock.Setup(m => m.Issue.Comment.Create(owner, name, issueNumber, comment))
            .ThrowsAsync(classFixture.OctokitException);

        FastForward.Infrastructure.Services.GitHubClient gitHubClient = new(octokitClientMock.Object);
        Func<Task<IssueComment>> action = () => gitHubClient.PostCommentAsync(owner, name, issueNumber, comment);

        (await action.Should().ThrowAsync<NotFoundException>()).Which.Should().BeSameAs(classFixture.OctokitException);
    }
}