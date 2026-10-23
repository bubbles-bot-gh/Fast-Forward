using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public class CommentBuilderUnitTests(ITestOutputHelper outputHelper, CommentBuilderUnitTestsFixture classFixture) : IClassFixture<CommentBuilderUnitTestsFixture>
{
    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentAcknowledgesAutoMergeEnabled(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
        
        actual.Should().ContainAll("Auto merge enabled", baseFullRef, headFullRef);
    }

    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentAcknowledgesAutoMergeDisabled(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        Mock.Get(provider.GetRequiredService<IActionOptions>())
            .SetupGet(m => m.IsAutoMerge).Returns(false);
        
        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
        
        actual.Should().ContainAll(
            "Auto merge disabled",
            baseFullRef,
            headFullRef);
    }

    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentAcknowledgesFastForwardIsNotPossible(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        Mock.Get(provider.GetRequiredService<IEventInfo>())
            .SetupGet(m => m.IsPossible).Returns(false);
        
        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
        
        actual.Should().ContainAll(
            "Can't fast-forward",
            baseFullRef,
            headFullRef);
    }

    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentShowsNoAncestorsWhenMergeBaseShaIsNull(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        Mock.Get(provider.GetRequiredService<IEventInfo>())
            .SetupGet(m => m.IsPossible).Returns(false);
        Mock.Get(provider.GetRequiredService<IPrInfo>())
            .SetupGet(m => m.MergeBaseSha).Returns((string?)null);

        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
    
        actual.Should().ContainAll(
            "Can't fast-forward",
            baseFullRef,
            headFullRef,
            "common ancestor");
    }

    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task MergeBaseParentsAmountIsZero_DoesNotExcludeAnyCommits(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        
        Mock<IEventInfo> eventInfoMock = Mock.Get(provider.GetRequiredService<IEventInfo>());
        eventInfoMock.SetupGet(m => m.IsPossible).Returns(false);
        
        Mock<IPrInfo> prInfoMock = Mock.Get(provider.GetRequiredService<IPrInfo>());        
        prInfoMock.SetupGet(m => m.MergeBaseSha).Returns("abc123");
        prInfoMock.SetupGet(m => m.MergeBaseParentsAmount).Returns(0);

        Mock<IGit> gitMock = Mock.Get(provider.GetRequiredService<IGit>());
        gitMock.Setup(m => m.LogCommitGraph(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync("graph output");

        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
    
        actual.Should().ContainAll(
            "Can't fast-forward",
            baseFullRef,
            headFullRef,
            "diverged at",
            "Rebase");

        gitMock.Verify(m => m.LogCommitGraph(
            string.Empty,
            actionInfo.PrInfo.BaseSha,
            actionInfo.PrInfo.HeadSha,
            It.IsAny<string>()),
            Times.Once);
    }

    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task MergeBaseParentsAmount_IsGreaterThanOne_ExcludesCommits(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        
        Mock<IEventInfo> eventInfoMock = Mock.Get(provider.GetRequiredService<IEventInfo>());
        eventInfoMock.SetupGet(m => m.IsPossible).Returns(false);
        
        Mock<IPrInfo> prInfoMock = Mock.Get(provider.GetRequiredService<IPrInfo>());        
        prInfoMock.SetupGet(m => m.MergeBaseSha).Returns("abc123");
        prInfoMock.SetupGet(m => m.MergeBaseParentsAmount).Returns(1);

        Mock<IGit> gitMock = Mock.Get(provider.GetRequiredService<IGit>());
        gitMock.Setup(m => m.LogCommitGraph(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync("graph output");

        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
    
        actual.Should().ContainAll(
            "Can't fast-forward",
            baseFullRef,
            headFullRef,
            "diverged at",
            "Rebase");

        gitMock.Verify(m => m.LogCommitGraph(
                "abc123",
                actionInfo.PrInfo.BaseSha,
                actionInfo.PrInfo.HeadSha,
                It.IsAny<string>()),
            Times.Once);
    }
    
    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentShowsAncestorsWhenMergeBaseShaExists(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        Mock.Get(provider.GetRequiredService<IEventInfo>())
            .SetupGet(m => m.IsPossible).Returns(false);
        Mock.Get(provider.GetRequiredService<IPrInfo>())
            .SetupGet(m => m.MergeBaseSha).Returns("abc123");

        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();

        IGit git = provider.GetRequiredService<IGit>();
        Mock.Get(git).Verify(
            m => m.LogCommitGraph(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    
        IPrInfo prInfo = provider.GetRequiredService<IPrInfo>();
        actual.Should().ContainAll(
            "Rebase",
            prInfo.BaseRef,
            prInfo.BaseSha,
            prInfo.HeadRef,
            prInfo.HeadSha,
            prInfo.MergeBaseSha,
            await git.LogCommitGraph(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()));
    }
    
    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentDoesNotShowAncestryWhenPossible(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
    
        actual.Should().NotContainAny(
            "Can't fast-forward",
            "ancestor");
    }
    
    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentAcknowledgesNoPerms(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        Mock.Get(provider.GetRequiredService<IEventInfo>())
            .Setup(m => m.UserHasPerms).ReturnsAsync(false);
        
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
    
        actual.Should().ContainAll(
            "@\u200B", // Zero-width character to avoid pinging the user
            "do not have permission to push to this repository",
            provider.GetRequiredService<IActionOptions>().CustomCommand);
    }
    
    [Theory]
    [MemberData(nameof(CommentBuilderUnitTestsFixture.TheoryData), MemberType = typeof(CommentBuilderUnitTestsFixture))]
    public async Task CommentAcknowledgesCommandNotInvoked(IServiceCollection services)
    {
        IServiceProvider provider = services.BuildServiceProvider();
        Mock.Get(provider.GetRequiredService<IEventInfo>())
            .SetupGet(m => m.CommandInvoked).Returns(false);

        IActionInfo actionInfo = provider.GetRequiredService<IActionInfo>();
        string baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
        string headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
        string actual = await provider.GetRequiredService<ICommentBuilder>().Build();
    
        actual.Should().ContainAll(
            baseFullRef,
            headFullRef,
            actionInfo.ActionOptions.CustomCommand,
            "initiate the fast-forward");
    }
}