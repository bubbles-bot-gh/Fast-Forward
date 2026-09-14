using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements;
using BubblesBotGitHub.FastForward.Implements.ActionInfo;
using BubblesBotGitHub.Tests.Unit.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Unit.Fixtures;

[UsedImplicitly]
public sealed class EventInfoFixture
{
    private static readonly string CustomCommand = AssemblyFixture.CustomCommand;
    private static readonly bool CommandInvoked = true;
    private static readonly bool IsCollaborator = true;

    // PullRequestOpenedEvent
    private static readonly PullRequestOpenedEvent PrOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly string PrOpenedCommentBody = PrOpenedEvent.PullRequest.Body 
        ?? throw new InvalidOperationException("Body unexpectedly null. Is event JSON correct?");

    public static TheoryData<IEventInfo, IEventInfo> TheoryData =>
    [
        new TheoryDataRow<IEventInfo, IEventInfo>(
            SetUpExpected(
                PrOpenedEvent, 
                ActionEventType.PullRequestOpened, 
                PrOpenedCommentBody), 
            SetUpActual(
                PrOpenedEvent, 
                ActionEventType.PullRequestOpened))
    ];

    private static IEventInfo SetUpExpected(WebhookEvent webhookEvent, ActionEventType eventType, string commentBody)
    {
        Mock<IEventInfo> mockEventInfo = new(MockBehavior.Strict);

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.CommentBody)
            .Returns(commentBody);

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.CommandInvoked)
            .Returns(CommandInvoked);
        
        MockServices mockServices = new()
        {
            MockEventInfo = mockEventInfo
        };

        IEventInfo expected = AssemblyFixture
            .CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices)
            .GetRequiredService<IEventInfo>();

        return expected;
    }

    private static IEventInfo SetUpActual(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        Mock<IActionOptions> mockOptions = new(MockBehavior.Strict);
        Mock<IRepoInfo> mockRepoInfo = new(MockBehavior.Strict);
        Mock<IGitHubApiCaller> mockGitHubApiCaller = new(MockBehavior.Strict);

        string owner = webhookEvent.Repository?.Owner.Login
            ?? throw new InvalidOperationException("Repository unexpectedly null. Is event JSON correct?");
        string name = webhookEvent.Repository.Name;
        string user = webhookEvent.Sender?.Login
            ?? throw new InvalidOperationException("Sender unexpectedly null. Is event JSON correct?");
        Console.WriteLine(user);

        mockOptions
            .SetupGet(options => options.CustomCommand)
            .Returns(CustomCommand);

        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.Owner)
            .Returns(owner);

        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.Name)
            .Returns(name);

        mockGitHubApiCaller
            .Setup(caller => caller.IsCollaborator(owner, name, user))
            .ReturnsAsync(() => IsCollaborator);
        
        MockServices mockServices = new()
        {
            MockActionOptions = mockOptions,
            MockRepoInfo = mockRepoInfo,
            MockGitHubApiCaller = mockGitHubApiCaller,
            MockEventInfo = null
        };
        
        IEventInfo actual = AssemblyFixture
            .CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices)
            .GetRequiredService<IEventInfo>();
        
        return actual;
    }
}