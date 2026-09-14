using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.ActionInfo;
using BubblesBotGitHub.FastForward.Implements.Git;
using BubblesBotGitHub.Tests.Unit.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.PrInfoTests;

[UsedImplicitly]
public class PrInfoFixture
{
    private static readonly PullRequestOpenedEvent PrOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    // internal IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.CreateIssueCommentCreatedEvent();
    // internal IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.CreateIssueCommentEditedEvent();

    private static readonly string BaseSha = PrOpenedEvent.PullRequest.Base.Sha;
    private static readonly string HeadSha = PrOpenedEvent.PullRequest.Head.Sha;
    private static readonly uint MergeBaseParents = 3;
    
    public static TheoryData<Func<IPrInfo>, Func<IPrInfo>> TheoryData => 
    [
        new TheoryDataRow<Func<IPrInfo>, Func<IPrInfo>>(
            ExpectedSetup(PrOpenedEvent, ActionEventType.PullRequestOpened), SubjectSetup(PrOpenedEvent, ActionEventType.PullRequestOpened))
            {
                Label = "WithPullRequestOpenedEvent",
                TestDisplayName = "WithPullRequestOpenedEvent"
            }
    ];

    private static Func<IPrInfo> ExpectedSetup(WebhookEvent webhookEvent, ActionEventType eventType) => () =>
    {
        Mock<IPrInfo> expected = new Mock<IPrInfo>(MockBehavior.Strict);
        
        expected
            .SetupGet(prInfo => prInfo.BaseRef)
            .Returns(PrOpenedEvent.PullRequest.Base.Ref);
        
        expected
            .SetupGet(prInfo => prInfo.BaseSha)
            .Returns(PrOpenedEvent.PullRequest.Base.Sha);
        
        expected
            .SetupGet(prInfo => prInfo.HeadRef)
            .Returns(PrOpenedEvent.PullRequest.Head.Ref);
        
        expected
            .SetupGet(prInfo => prInfo.HeadSha)
            .Returns(PrOpenedEvent.PullRequest.Head.Sha);
        
        expected
            .SetupGet(prInfo => prInfo.HeadLabel)
            .Returns(PrOpenedEvent.PullRequest.Head.Label);
        
        expected
            .SetupGet(prInfo => prInfo.MergeBaseSha)
            .Returns(BaseSha);
        
        expected
            .SetupGet(prInfo => prInfo.MergeBaseParentsAmount)
            .Returns(MergeBaseParents);
        
        MockServices mockServices = new() { MockPrInfo = expected, MockActionInfo = null, MockActionInfoFactory = null };
        IPrInfo subject = AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices)
            .GetRequiredService<IPrInfo>();

        return subject;
    };

    private static Func<IPrInfo> SubjectSetup(WebhookEvent webhookEvent, ActionEventType eventType) => () =>
    {
        Mock<IGit> mockGit = new Mock<IGit>(MockBehavior.Strict);
        Mock<IGitHubApiCaller> mockGitHubApiCaller = new Mock<IGitHubApiCaller>(MockBehavior.Strict);
        
        mockGit
            .Setup(git => git.GetMergeBaseSha(BaseSha, HeadSha))
            .ReturnsAsync(BaseSha);
        
        mockGit
            .Setup(git => git.GetAmountOfParents(BaseSha))
            .ReturnsAsync(MergeBaseParents);
        
        MockServices mockServices = new() { MockGit = mockGit, MockPrInfo = null, MockActionInfo = null, MockActionInfoFactory = null };
        IPrInfo subject = AssemblyFixture
            .CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices)
            .GetRequiredService<IPrInfo>();
        
        subject.InitializeAsync(mockGit.Object, mockGitHubApiCaller.Object, webhookEvent, eventType);

        return subject;
    };
}