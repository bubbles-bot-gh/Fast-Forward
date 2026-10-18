using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
    // All events either contain a pull request or retrieve pull request data from the GitHub API. Thus, they are all
    // functionally the same. This could change with future supported events.
    public static TheoryData<IServiceCollection> TheoryData =>
    [
        new TheoryDataRow<IServiceCollection>(SetEventData(AssemblyFixture.GetPullRequestOpenedEvent(), 
            ActionEventType.PullRequestOpened))
    ];

    private static IServiceCollection SetEventData(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        PullRequestOpenedEvent @event = webhookEvent as PullRequestOpenedEvent 
            ?? throw new InvalidOperationException("Could not cast WebhookEvent to 'PullRequestOpenedEvent'");
        
        string baseRef = @event.PullRequest.Base.Ref;
        string baseSha = @event.PullRequest.Base.Sha;
        string headRef = @event.PullRequest.Head.Ref;
        string headSha = @event.PullRequest.Head.Sha;
        string headRepoOwner = @event.PullRequest.Head.Repo?.Owner.Login ?? throw new InvalidOperationException();
        string headRepoName = @event.PullRequest.Head.Repo.Name;
        string? mergeBaseSha = @event.PullRequest.MergeCommitSha;
        const uint mergeBaseParentsAmount = 0;
        string baseRepoOwner = @event.PullRequest.Base.Repo.Owner.Login;
        string baseRepoName = @event.PullRequest.Base.Repo.Name;
        
        Mock<IActionOptions> actionOptionsMock = SetUpActionOptionsMock(
            customCommand: "/fast-forward",
            isAutoMerge: true,
            postComment: "always");

        Mock<IPrInfo> prInfoMock = SetUpPrInfoMock(
            baseRef,
            baseSha,
            headRef,
            headSha,
            headRepoOwner,
            headRepoName,
            mergeBaseSha,
            mergeBaseParentsAmount);

        Mock<IRepoInfo> repoInfoMock = SetUpRepoInfoMock(baseRepoOwner, baseRepoName);
        
        Mock<IEventInfo> eventInfoMock = SetUpEventInfoMock(
            isPossible: true,
            userHasPerms: true,
            commandInvoked: true,
            userName: "luneisolei",
            shouldExit: false);

        Mock<IActionInfo> actionInfoMock = SetUpActionInfoMock(
            actionOptionsMock,
            eventInfoMock,
            prInfoMock,
            repoInfoMock);

        MockServices mockServices = new MockServices
        {
            MockActionOptions = actionOptionsMock,
            MockPrInfo = prInfoMock,
            MockRepoInfo = repoInfoMock,
            MockEventInfo = eventInfoMock,
            MockGitHubClient = SetUpGitHubClientMock(commit: AssemblyFixture.GetGitHubCommit()),
            MockGit = SetUpGitMock(graphOutput: It.IsAny<string>()),
            MockActionInfo = actionInfoMock,
            MockCommentBuilder = null
        };
        
        return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices);
    }

    private static Mock<IActionOptions> SetUpActionOptionsMock(string customCommand, bool isAutoMerge, string postComment)
    {
        Mock<IActionOptions> actionOptionsMock = new(MockBehavior.Strict);
        
        actionOptionsMock.SetupGet(m => m.CustomCommand).Returns(customCommand);
        actionOptionsMock.SetupGet(m => m.IsAutoMerge).Returns(isAutoMerge);
        actionOptionsMock.SetupGet(m => m.PostComment).Returns(postComment);

        return actionOptionsMock;
    }

    private static Mock<IPrInfo> SetUpPrInfoMock(string baseRef, 
        string baseSha, 
        string headRef, 
        string headSha, 
        string headRepoOwner, 
        string headRepoName, 
        string mergeBaseSha, 
        uint mergeBaseParentsAmount)
    {
        Mock<IPrInfo> prInfoMock = new(MockBehavior.Strict);
        
        prInfoMock.SetupGet(m => m.BaseRef).Returns(baseRef);
        prInfoMock.SetupGet(m => m.BaseSha).Returns(baseSha);
        prInfoMock.SetupGet(m => m.HeadRef).Returns(headRef);
        prInfoMock.SetupGet(m => m.HeadSha).Returns(headSha);
        prInfoMock.SetupGet(m => m.HeadRepoOwner).Returns(headRepoOwner);
        prInfoMock.SetupGet(m => m.HeadRepoName).Returns(headRepoName);
        prInfoMock.SetupGet(m => m.MergeBaseSha).Returns(mergeBaseSha);
        prInfoMock.SetupGet(m => m.MergeBaseParentsAmount).Returns(mergeBaseParentsAmount);

        return prInfoMock;
    }

    private static Mock<IRepoInfo> SetUpRepoInfoMock(string baseRepoOwner, string baseRepoName)
    {
        Mock<IRepoInfo> repoInfoMock = new(MockBehavior.Strict);
        
        repoInfoMock.SetupGet(m => m.Owner).Returns(baseRepoOwner);
        repoInfoMock.SetupGet(m => m.Name).Returns(baseRepoName);

        return repoInfoMock;
    }

    private static Mock<IEventInfo> SetUpEventInfoMock(bool isPossible, 
        bool userHasPerms, 
        bool commandInvoked, 
        string userName, 
        bool shouldExit)
    {
        Mock<IEventInfo> eventInfoMock = new(MockBehavior.Strict);
        
        eventInfoMock.SetupGet(m => m.IsPossible).Returns(isPossible);
        eventInfoMock.SetupGet(m => m.UserHasPerms).Returns(Task.FromResult(userHasPerms));
        eventInfoMock.SetupGet(m => m.CommandInvoked).Returns(commandInvoked);
        eventInfoMock.SetupGet(m => m.User).Returns(userName);
        eventInfoMock.SetupGet(m => m.ShouldExit).Returns(shouldExit);
        eventInfoMock.SetupSet(m => m.ShouldExit = It.IsAny<bool>());

        return eventInfoMock;
    }

    private static Mock<IGitHubClient> SetUpGitHubClientMock(GitHubCommit commit)
    {
        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        
        gitHubClientMock.Setup(m => m.GetCommit(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(commit);

        return gitHubClientMock;
    }

    private static Mock<IGit> SetUpGitMock(string graphOutput)
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

    private static Mock<IActionInfo> SetUpActionInfoMock(Mock<IActionOptions> actionOptionsMock,
        Mock<IEventInfo> eventInfoMock,
        Mock<IPrInfo> prInfoMock,
        Mock<IRepoInfo> repoInfoMock)
    {
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