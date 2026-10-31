using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Octokit;
using Octokit.Webhooks;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.IGitHubClient;
#pragma warning disable CS8618

namespace BubblesBotGitHub.Tests.Fixtures;

[UsedImplicitly]
public class CommentBuilderTestFixture
{
    private const string BaseRef = "main";
    public const string BaseSha = "bb4b8199b437efb9a596d4984c2e8f14fc6597a482a40248a67b972661c97219";
    private const string HeadRef = "feat/user-login";
    public const string HeadSha = "63fdb1871623d514c839b17af2599d4b07a0333a3bda5f805e6c90faa0ceb457";
    private const string BaseRepoOwner = "bubbles-bot-gh";
    private const string BaseRepoName = "Fast-Forward";
    private const string HeadRepoOwner = "luneisolei";
    private const string HeadRepoName = "fast-forward-fork";
    public const string MergeBaseSha = "e57bbb27341462cb23ef19ff411310af1fed4ea4bb2de4c4350eae7e541e69fe";
    private const uint ParentCount = 1;
    private const bool IsPossible = true;
    private const bool UserHasPerms = true;
    private const bool CommandInvoked = true;
    private const string UserName = "luneisolei";
    private const bool ShouldExit = false;
    public const string CustomCommand = "/custom-command";
    private const bool IsAutoMerge = true;
    private const string PostComment = "always";
    public const string GraphOutput = "some-valid-graph-output";
    private static readonly GitHubCommit Commit = AssemblyFixture.GetGitHubCommit();
    private static readonly WebhookEvent WebhookEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private const ActionEventType EventType = ActionEventType.PullRequestOpened;

    public readonly Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder>
        CreateApplicationBuilderWithMocks = AssemblyFixture.CreateApplicationBuilderWithMocks;

    public async Task<string> BuildComment(
        bool isAutoMerge = IsAutoMerge, 
        bool isPossible = IsPossible,
        string? mergeBaseSha = MergeBaseSha,
        uint parentCount = ParentCount,
        bool userHasPerms = UserHasPerms,
        bool commandInvoked = CommandInvoked,
        Mock<IGit>? gitMock = null,
        Mock<IPrInfo>? prInfoMock = null)
    {
        prInfoMock ??= CreatePrInfoMock(mergeBaseSha: mergeBaseSha, parentCount: parentCount);
        gitMock ??= CreateGitMock();
        Mock<IActionOptions> actionOptionsMock = CreateActionOptionsMock(isAutoMerge: isAutoMerge);
        Mock<IEventInfo> eventInfoMock = CreateEventInfoMock(isPossible: isPossible, userHasPerms: userHasPerms, commandInvoked: commandInvoked);
        Mock<IRepoInfo> repoInfoMock = CreateRepoInfoMock();
        
        using IHost host = AssemblyFixture.CreateApplicationBuilderWithMocks(
            WebhookEvent,
            EventType,
            new MockServices
            {
                MockGitHubClient = CreateGitHubClientMock(),
                MockGit = gitMock,
                MockActionOptions = actionOptionsMock,
                MockEventInfo = eventInfoMock,
                MockRepoInfo = repoInfoMock,
                MockPrInfo = prInfoMock,
                MockActionInfo = CreateActionInfoMock(actionOptionsMock, eventInfoMock, prInfoMock, repoInfoMock),
                MockCommentBuilder = null
            }).Build();
        
        return await host.Services.GetRequiredService<ICommentBuilder>().Build();
    }

    public void VerifyLogCommitGraphCall(Mock<IGit> gitMock, string exclude = MergeBaseSha)
    {
        gitMock.Verify(
            m => m.LogCommitGraph(exclude, BaseSha, HeadSha, It.IsAny<string>()), Times.Once);
    }

    private Mock<IActionOptions> CreateActionOptionsMock(
        string customCommand = CustomCommand, bool isAutoMerge = IsAutoMerge, string postComment = PostComment)
    {
        Mock<IActionOptions> actionOptionsMock = new(MockBehavior.Strict);
        
        actionOptionsMock.SetupGet(m => m.CustomCommand).Returns(customCommand);
        actionOptionsMock.SetupGet(m => m.IsAutoMerge).Returns(isAutoMerge);
        actionOptionsMock.SetupGet(m => m.PostComment).Returns(postComment);

        return actionOptionsMock;
    }

    public Mock<IPrInfo> CreatePrInfoMock(
        string baseRef = BaseRef, 
        string baseSha = BaseSha, 
        string headRef = HeadRef, 
        string headSha = HeadSha, 
        string headRepoOwner = HeadRepoOwner, 
        string headRepoName = HeadRepoName, 
        string? mergeBaseSha = MergeBaseSha, 
        uint parentCount = ParentCount)
    {
        Mock<IPrInfo> prInfoMock = new(MockBehavior.Strict);
        
        prInfoMock.SetupGet(m => m.BaseRef).Returns(baseRef);
        prInfoMock.SetupGet(m => m.BaseSha).Returns(baseSha);
        prInfoMock.SetupGet(m => m.HeadRef).Returns(headRef);
        prInfoMock.SetupGet(m => m.HeadSha).Returns(headSha);
        prInfoMock.SetupGet(m => m.HeadRepoOwner).Returns(headRepoOwner);
        prInfoMock.SetupGet(m => m.HeadRepoName).Returns(headRepoName);
        prInfoMock.SetupGet(m => m.MergeBaseSha).Returns(mergeBaseSha);
        prInfoMock.SetupGet(m => m.MergeBaseParentsAmount).Returns(parentCount);

        return prInfoMock;
    }

    private Mock<IRepoInfo> CreateRepoInfoMock(string baseRepoOwner = BaseRepoOwner, string baseRepoName = BaseRepoName)
    {
        Mock<IRepoInfo> repoInfoMock = new(MockBehavior.Strict);
        
        repoInfoMock.SetupGet(m => m.Owner).Returns(baseRepoOwner);
        repoInfoMock.SetupGet(m => m.Name).Returns(baseRepoName);

        return repoInfoMock;
    }

    private Mock<IEventInfo> CreateEventInfoMock(
        bool isPossible = IsPossible, 
        bool userHasPerms = UserHasPerms, 
        bool commandInvoked = CommandInvoked, 
        string userName = UserName, 
        bool shouldExit = ShouldExit)
    {
        Mock<IEventInfo> eventInfoMock = new(MockBehavior.Strict);
        
        eventInfoMock.Setup(m => m.IsPossible).ReturnsAsync(isPossible);
        eventInfoMock.SetupGet(m => m.UserHasPerms).Returns(Task.FromResult(userHasPerms));
        eventInfoMock.SetupGet(m => m.CommandInvoked).Returns(commandInvoked);
        eventInfoMock.SetupGet(m => m.User).Returns(userName);
        eventInfoMock.SetupGet(m => m.ShouldExit).Returns(shouldExit);
        eventInfoMock.SetupSet(m => m.ShouldExit = It.IsAny<bool>());

        return eventInfoMock;
    }

    private Mock<IGitHubClient> CreateGitHubClientMock(GitHubCommit? commit = null)
    {
        commit ??= Commit;
        
        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        
        gitHubClientMock.Setup(m => m.GetCommitAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(commit);

        return gitHubClientMock;
    }

    public Mock<IGit> CreateGitMock(string graphOutput = GraphOutput)
    {
        Mock<IGit> gitMock = new(MockBehavior.Strict);
        
        gitMock.Setup(m => m.LogCommitGraph(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(graphOutput);

        return gitMock;
    }

    private Mock<IActionInfo> CreateActionInfoMock(
        Mock<IActionOptions>? actionOptionsMock = null,
        Mock<IEventInfo>? eventInfoMock = null,
        Mock<IPrInfo>? prInfoMock = null,
        Mock<IRepoInfo>? repoInfoMock = null)
    {
        actionOptionsMock ??= CreateActionOptionsMock();
        eventInfoMock ??= CreateEventInfoMock();
        prInfoMock ??= CreatePrInfoMock();
        repoInfoMock ??= CreateRepoInfoMock();
        
        Mock<IActionInfo> actionInfoMock = new(MockBehavior.Strict);

        actionInfoMock.Setup(m => m.ActionOptions).Returns(actionOptionsMock.Object);
        actionInfoMock.Setup(m => m.EventInfo).Returns(eventInfoMock.Object);
        actionInfoMock.Setup(m => m.PrInfo).Returns(prInfoMock.Object);
        actionInfoMock.Setup(m => m.RepoInfo).Returns(repoInfoMock.Object);

        return actionInfoMock;
    }

    private static Mock<IOptions<Config>> SetUpConfigMock()
    {
        Mock<IOptions<Config>> configMock = new(MockBehavior.Strict);
    
        return configMock;
    }
}