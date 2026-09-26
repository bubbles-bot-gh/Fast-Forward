using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Moq;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public class CommentBuilderUnitTests : IClassFixture<CommentBuilderUnitTestsFixture>
{
    private readonly CommentBuilderUnitTestsFixture _classFixture;
    private ITestOutputHelper _output;
    
    public CommentBuilderUnitTests(ITestOutputHelper output, CommentBuilderUnitTestsFixture classFixture)
    {
        classFixture.Reset();
        _classFixture = classFixture;
        _output = output;
    }
    
    [Fact]
    public async Task CommentAcknowledgesAutoMergeEnabled()
    {
        ICommentBuilder subject = _classFixture.CreateSubject();
        string baseFullRef = $"{_classFixture.BaseRef} ({_classFixture.BaseSha})";
        string headFullRef = $"{_classFixture.HeadRef} ({_classFixture.HeadSha})";
        string actual = await subject.Build();

        actual.Should().ContainAll(
            "Auto merge enabled",
            baseFullRef,
            headFullRef);
    }

    [Fact]
    public async Task CommentAcknowledgesAutoMergeDisabled()
    {
        _classFixture.IsAutoMerge = false;
        ICommentBuilder subject = _classFixture.CreateSubject();
        string baseFullRef = $"{_classFixture.BaseRef} ({_classFixture.BaseSha})";
        string headFullRef = $"{_classFixture.HeadRef} ({_classFixture.HeadSha})";
        string actual = await subject.Build();

        actual.Should().ContainAll(
            "Auto merge disabled",
            baseFullRef,
            headFullRef);
    }

    [Fact]
    public async Task CommentAcknowledgesFastForwardIsNotPossible()
    {
        _classFixture.IsPossible = false;
        ICommentBuilder subject = _classFixture.CreateSubject();
        string actual = await subject.Build();
        string baseFullRef = $"{_classFixture.BaseRef} ({_classFixture.BaseSha})";
        string headFullRef = $"{_classFixture.HeadRef} ({_classFixture.HeadSha})";

        actual.Should().ContainAll(
            "Can't fast-forward",
            baseFullRef,
            headFullRef);
    }

    [Fact]
    public async Task CommentShowsNoAncestorsWhenMergeBaseShaIsNull()
    {
        _classFixture.IsPossible = false;
        _classFixture.MergeBaseSha = null;
        ICommentBuilder subject = _classFixture.CreateSubject();
        string actual = await subject.Build();
        string baseFullRef = $"{_classFixture.BaseRef} ({_classFixture.BaseSha})";
        string headFullRef = $"{_classFixture.HeadRef} ({_classFixture.HeadSha})";

        actual.Should().ContainAll(
            "Can't fast-forward",
            baseFullRef,
            headFullRef,
            "common ancestor");
    }

    [Fact]
    public async Task CommentShowsAncestorsWhenMergeBaseShaExists()
    {
        _classFixture.IsPossible = false;
        _classFixture.MergeBaseSha = "abc123";
        ICommentBuilder subject = _classFixture.CreateSubject();
        string actual = await subject.Build();

        _classFixture.GitMock.Verify(
            m => m.LogCommitGraph(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);

        actual.Should().ContainAll(
            "Rebase",
            _classFixture.BaseRef,
            _classFixture.BaseSha,
            _classFixture.HeadRef,
            _classFixture.HeadSha,
            _classFixture.MergeBaseSha,
            _classFixture.GraphOutput);
    }

    [Fact]
    public async Task CommentDoesNotShowAncestryWhenPossible()
    {
        ICommentBuilder subject = _classFixture.CreateSubject();
        string actual = await subject.Build();

        actual.Should().NotContainAny(
            "Can't fast-forward",
            "ancestor");
    }

    [Fact]
    public async Task CommentAcknowledgesNoPerms()
    {
        _classFixture.UserHasPerms = false;
        ICommentBuilder subject = _classFixture.CreateSubject();
        string actual = await subject.Build();

        actual.Should().ContainAll(
            "@\u200B", // Zero-width character to avoid pinging the user
            "do not have permission to push to this repository",
            _classFixture.CustomCommand);
    }

    [Fact]
    public async Task CommentAcknowledgesCommandNotInvoked()
    {
        _classFixture.CommandInvoked = false;
        ICommentBuilder subject = _classFixture.CreateSubject();
        string actual = await subject.Build();
        string baseFullRef = $"{_classFixture.BaseRef} ({_classFixture.BaseSha})";
        string headFullRef = $"{_classFixture.HeadRef} ({_classFixture.HeadSha})";

        actual.Should().ContainAll(
            baseFullRef,
            headFullRef,
            _classFixture.CustomCommand,
            "initiate the fast-forward");
    }
}