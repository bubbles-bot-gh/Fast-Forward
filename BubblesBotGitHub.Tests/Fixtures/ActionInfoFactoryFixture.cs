using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

public sealed class ActionInfoFactoryFixture
{
    private static readonly PullRequestOpenedEvent PullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    private static readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();

    public static TheoryData<WebhookEvent, ActionEventType> TheoryData =>
    [
        new TheoryDataRow<WebhookEvent, ActionEventType>(PullRequestOpenedEvent, ActionEventType.PullRequestOpened),
        new TheoryDataRow<WebhookEvent, ActionEventType>(IssueCommentCreatedEvent, ActionEventType.IssueCommentCreated),
        new TheoryDataRow<WebhookEvent, ActionEventType>(IssueCommentEditedEvent, ActionEventType.IssueCommentEdited)
    ];

    public IServiceCollection CreateServiceCollectionWithMocks(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        MockServices serviceMocks = new()
        {
            MockActionInfoFactory = null,
            MockActionInfo = null
        };
        
        return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, serviceMocks);
    }
}