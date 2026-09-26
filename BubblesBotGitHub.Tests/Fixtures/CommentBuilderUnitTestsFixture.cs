using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.PullRequest;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;
#pragma warning disable CS8618

namespace BubblesBotGitHub.Tests.Fixtures;

[UsedImplicitly]
public class CommentBuilderUnitTestsFixture
{
    public readonly PullRequestOpenedEvent TestEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    public Mock<IPrInfo> PrInfoMock { get; } = new(MockBehavior.Strict);
    public Mock<IEventInfo> EventInfoMock { get; } = new(MockBehavior.Strict);
    public Mock<IActionOptions> ActionOptionsMock { get; } = new(MockBehavior.Strict);
    public Mock<IRepoInfo> RepoInfoMock { get; } = new(MockBehavior.Strict);
    public Mock<IGitHubClient> GitHubClientMock { get; } = new(MockBehavior.Strict);
    public Mock<IGit> GitMock { get; } = new(MockBehavior.Strict);

    public string BaseRef;
    public string BaseSha;
    public string HeadRef;
    public string HeadSha;
    public string HeadRepoOwner;
    public string HeadRepoName;
    public string? MergeBaseSha;
    public uint MergeBaseParentsAmount;
    public string BaseRepoOwner;
    public string BaseRepoName;
    public bool IsPossible;
    public bool UserHasPerms;
    public bool CommandInvoked;
    public string UserName;
    public bool ShouldExit;
    public GitHubCommit Commit;
    public string GraphOutput;
    public string CustomCommand;
    public bool IsAutoMerge;
    public string PostComment;

    public void Reset()
    {
        BaseRef = TestEvent.PullRequest.Base.Ref;
        BaseSha = TestEvent.PullRequest.Base.Sha;
        HeadRef = TestEvent.PullRequest.Head.Ref;
        HeadSha = TestEvent.PullRequest.Head.Sha;
        HeadRepoOwner = TestEvent.PullRequest.Head.Repo?.Owner.Login ?? throw new InvalidOperationException();
        HeadRepoName = TestEvent.PullRequest.Head.Repo.Name;
        MergeBaseSha = TestEvent.PullRequest.MergeCommitSha;
        BaseRepoOwner = TestEvent.PullRequest.Base.Repo.Owner.Login;
        BaseRepoName = TestEvent.PullRequest.Base.Repo.Name;

        MergeBaseParentsAmount = 0;
        IsPossible = true;
        UserHasPerms = true;
        CommandInvoked = true;
        UserName = "luneisolei";
        ShouldExit = false;
        Commit = AssemblyFixture.GetGitHubCommit();
        GraphOutput = "This is an example graph output from git.";
        CustomCommand = "/fast-forward";
        IsAutoMerge = true;
        PostComment = "always";
    }

    public CommentBuilderUnitTestsFixture()
    {
        Reset();
    }

    public ICommentBuilder CreateSubject()
    {
        ActionOptionsMock.SetupGet(m => m.CustomCommand).Returns(CustomCommand);
        ActionOptionsMock.SetupGet(m => m.IsAutoMerge).Returns(IsAutoMerge);
        ActionOptionsMock.SetupGet(m => m.PostComment).Returns(PostComment);
        
        PrInfoMock.SetupGet(m => m.BaseRef).Returns(BaseRef);
        PrInfoMock.SetupGet(m => m.BaseSha).Returns(BaseSha);
        PrInfoMock.SetupGet(m => m.HeadRef).Returns(HeadRef);
        PrInfoMock.SetupGet(m => m.HeadSha).Returns(HeadSha);
        PrInfoMock.SetupGet(m => m.HeadRepoOwner).Returns(HeadRepoOwner);
        PrInfoMock.SetupGet(m => m.HeadRepoName).Returns(HeadRepoName);
        PrInfoMock.SetupGet(m => m.MergeBaseSha).Returns(MergeBaseSha);
        PrInfoMock.SetupGet(m => m.MergeBaseParentsAmount).Returns(MergeBaseParentsAmount);

        RepoInfoMock.SetupGet(m => m.Owner).Returns(BaseRepoOwner);
        RepoInfoMock.SetupGet(m => m.Name).Returns(BaseRepoName);

        EventInfoMock.SetupGet(m => m.IsPossible).Returns(IsPossible);
        EventInfoMock.SetupGet(m => m.UserHasPerms).Returns(Task.FromResult(UserHasPerms));
        EventInfoMock.SetupGet(m => m.CommandInvoked).Returns(CommandInvoked);
        EventInfoMock.SetupGet(m => m.User).Returns(UserName);
        EventInfoMock.SetupGet(m => m.ShouldExit).Returns(ShouldExit);
        EventInfoMock.SetupSet(m => m.ShouldExit = It.IsAny<bool>());

        GitHubClientMock.Setup(m => m.GetCommit(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Commit);

        GitMock.Setup(m => m.LogCommitGraph(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ReturnsAsync(GraphOutput);
        
        Mock<IActionInfo> actionInfoMock = new(MockBehavior.Strict);
        actionInfoMock.SetupGet(m => m.PrInfo).Returns(PrInfoMock.Object);
        actionInfoMock.SetupGet(m => m.RepoInfo).Returns(RepoInfoMock.Object);
        actionInfoMock.SetupGet(m => m.EventInfo).Returns(EventInfoMock.Object);
        actionInfoMock.SetupGet(m => m.ActionOptions).Returns(ActionOptionsMock.Object);
        
        return new CommentBuilder(actionInfoMock.Object, GitHubClientMock.Object, GitMock.Object);
    }
}