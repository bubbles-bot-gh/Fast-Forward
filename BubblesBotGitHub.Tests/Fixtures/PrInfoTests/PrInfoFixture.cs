using System.Text.Json;
using System.Text.Json.Serialization;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures.PrInfoTests;

[UsedImplicitly]
public class PrInfoFixture
{
    private static readonly PullRequestOpenedEvent PrOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    private static readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();

    private static readonly string BaseSha = PrOpenedEvent.PullRequest.Base.Sha;
    private static readonly string HeadSha = PrOpenedEvent.PullRequest.Head.Sha;
    private static readonly uint MergeBaseParents = 3;
    
    public static TheoryData<Func<IPrInfo>, Func<Task<IPrInfo>>> TheoryData => 
    [
        new TheoryDataRow<Func<IPrInfo>, Func<Task<IPrInfo>>>(
            ExpectedSetup(
                PrOpenedEvent, 
                ActionEventType.PullRequestOpened), 
            SubjectSetup(PrOpenedEvent, ActionEventType.PullRequestOpened))
            {
                Label = "WithPullRequestOpenedEvent",
                TestDisplayName = "WithPullRequestOpenedEvent"
            },
        new TheoryDataRow<Func<IPrInfo>, Func<Task<IPrInfo>>>(
            ExpectedSetup(
                IssueCommentCreatedEvent, 
                ActionEventType.IssueCommentCreated), 
            SubjectSetup(IssueCommentCreatedEvent, ActionEventType.IssueCommentCreated))
            {
                Label = "WithIssueCommentCreatedEvent",
                TestDisplayName = "WithIssueCommentCreatedEvent"
            },
        new TheoryDataRow<Func<IPrInfo>, Func<Task<IPrInfo>>>(
            ExpectedSetup(
                IssueCommentEditedEvent,
                ActionEventType.IssueCommentEdited),
            SubjectSetup(IssueCommentEditedEvent, ActionEventType.IssueCommentEdited))
            {
                Label = "WithIssueCommentEditedEvent",
                TestDisplayName = "WithIssueCommentEditedEvent"
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
            .BuildServiceProvider()
            .GetRequiredService<IPrInfo>();

        return subject;
    };

    private static Func<Task<IPrInfo>> SubjectSetup(WebhookEvent webhookEvent, ActionEventType eventType) => async Task<IPrInfo> () =>
    {
        Mock<IGit> mockGit = new Mock<IGit>(MockBehavior.Strict);
        Mock<IGitHubClient> mockGitHubClient = new Mock<IGitHubClient>(MockBehavior.Strict);
        
        mockGit
            .Setup(git => git.GetMergeBaseSha(BaseSha, HeadSha, It.IsAny<string>()))
            .ReturnsAsync(BaseSha);
        
        mockGit
            .Setup(git => git.GetAmountOfParents(BaseSha, It.IsAny<string>()))
            .ReturnsAsync(MergeBaseParents);

        PullRequestOpenedEvent prEvent = AssemblyFixture.GetPullRequestOpenedEvent();
        JsonSerializerOptions jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        string json = JsonSerializer.Serialize(prEvent.PullRequest, jsonOptions);
        PullRequest pr = new Octokit.Internal
                .SimpleJsonSerializer()
                .Deserialize<PullRequest>(json)
            ?? throw new InvalidOperationException("Could not deserialize PullRequest");
        mockGitHubClient.Setup(m => m.GetPullRequestAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>()))
            .ReturnsAsync(pr);
        
        MockServices mockServices = new()
        {
            MockGit = mockGit, 
            MockGitHubClient = mockGitHubClient,
            MockPrInfo = null, 
            MockActionInfo = null, 
            MockActionInfoFactory = null
        };
        IPrInfo subject = AssemblyFixture
            .CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices)
            .BuildServiceProvider()
            .GetRequiredService<IPrInfo>();
        
        await subject.InitializeAsync(mockGit.Object, mockGitHubClient.Object, webhookEvent, eventType);

        return subject;
    };
}