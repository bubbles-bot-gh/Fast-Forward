using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services;

[Trait(Traits.Names.Category, Traits.Categories.Names.CommentBuilderTests)]
public sealed class Build(CommentBuilderTestFixture classFixture) : IClassFixture<CommentBuilderTestFixture>
{
    [Fact]
    public async Task AutoMergeEnabled_ReturnsStringWithEnabledStatus()
    {
        string comment = await classFixture.BuildComment();
        comment.Should().Contain("Auto merge enabled");
    }

    [Fact]
    public async Task AutoMergeDisabled_ReturnsStringWithDisabledStatus()
    {
        string comment = await classFixture.BuildComment(isAutoMerge: false);
        comment.Should().Contain("Auto merge disabled");
    }

    [Fact]
    public async Task FastForwardNotPossible_ReturnsStringWithFastForwardStatus()
    {
        string comment = await classFixture.BuildComment(isPossible: false);
        comment.Should().Contain("Can't fast-forward");
    }

    [Fact]
    public async Task NullMergeBaseSha_ReturnsStringWithNoAncestorStatus()
    {
        string comment = await classFixture.BuildComment(isPossible: false, mergeBaseSha: null);
        comment.Should().Contain("Branches do not appear to have a common ancestor");
    }

    [Fact]
    public async Task ParentCountZero_ReturnsStringWithNoExcludedCommits()
    {
        Mock<IGit> gitMock = classFixture.CreateGitMock();
        string comment = await classFixture.BuildComment(
            isPossible: false, 
            mergeBaseSha: "abc123", 
            parentCount: 0,
            gitMock: gitMock);
        
        classFixture.VerifyLogCommitGraphCall(gitMock, string.Empty);
        comment.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ParentCountGreaterThanOne_ReturnsStringWithExcludedCommits()
    {
        const string exclude = "abc123";
        
        Mock<IGit> gitMock = classFixture.CreateGitMock();
        
        string comment = await classFixture.BuildComment(
            isPossible: false, 
            mergeBaseSha: exclude, 
            parentCount: 1,
            gitMock: gitMock);

        classFixture.VerifyLogCommitGraphCall(gitMock, exclude);
        comment.Should().Contain(CommentBuilderTestFixture.GraphOutput);
    }

    [Fact]
    public async Task MergeBaseShaExists_ReturnsStringWithAncestors()
    {
        Mock<IGit> gitMock = classFixture.CreateGitMock();
        Mock<IPrInfo> prInfoMock = classFixture.CreatePrInfoMock();
        
        string comment = await classFixture.BuildComment(
            isPossible: false, gitMock: gitMock);
        
        classFixture.VerifyLogCommitGraphCall(gitMock);

        IPrInfo prInfo = prInfoMock.Object;
        comment.Should().ContainAll(
            prInfo.BaseRef,
            prInfo.BaseSha,
            prInfo.HeadRef,
            prInfo.HeadSha,
            prInfo.MergeBaseSha);
    }

    [Fact]
    public async Task PossibleFastForward_DoesNotShowAncestry()
    {
        string comment = await classFixture.BuildComment();
        comment.Should().NotContainAny("Can't fast-forward", "ancestor");
    }

    [Fact]
    public async Task NoPermissions_ReturnsStringWithNoPermissionsStatus()
    {
        string comment = await classFixture.BuildComment(userHasPerms: false);
        comment.Should().ContainAll(
            "@\u200B",
            "do not have permission to push to this repository",
            CommentBuilderTestFixture.CustomCommand);
    }

    [Fact]
    public async Task CommandNotInvoked_ReturnsStringWithNotInvokedStatus()
    {
        string comment = await classFixture.BuildComment(commandInvoked: false);
        comment.Should().ContainAll(CommentBuilderTestFixture.CustomCommand, "initiate the fast-forward");
    }
}