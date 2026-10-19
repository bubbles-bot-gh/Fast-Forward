using System.Text.Json;
using System.Text.Json.Nodes;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

[UsedImplicitly]
public sealed class EventInfoFixture
{
    private static readonly string DefaultCustomCommand = "/fast-forward";
    private static readonly bool CommandInvoked = true;
    private static readonly bool DefaultIsCollaborator = true;
    private static readonly PullRequestOpenedEvent PullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    private static readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();
    
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData =>
    [
        new TheoryDataRow<WebhookEvent, ActionEventType>(PullRequestOpenedEvent, ActionEventType.PullRequestOpened),
        new TheoryDataRow<WebhookEvent, ActionEventType>(IssueCommentCreatedEvent, ActionEventType.IssueCommentCreated),
        new TheoryDataRow<WebhookEvent, ActionEventType>(IssueCommentEditedEvent, ActionEventType.IssueCommentEdited)
    ];

    public IServiceProvider CreateServiceProviderWithMocks(
        WebhookEvent webhookEvent, 
        ActionEventType eventType, 
        MockServices? mockServices = null)
    {
        // Check for overrides
        if (mockServices is not null)
            return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, mockServices)
                .BuildServiceProvider();
        
        // Use default mocks
        MockServices services = new()
        {
            MockActionOptions = new(MockBehavior.Strict),
            MockRepoInfo = new(MockBehavior.Strict),
            MockGitHubClient = new(MockBehavior.Strict),
            MockEventInfo = null
        };

        // Set up ActionOptions mock
        services.MockActionOptions.Setup(m => m.CustomCommand).Returns(DefaultCustomCommand);
        
        // Set up RepoInfo mock
        string owner = webhookEvent.Repository?.Owner.Login 
            ?? throw new InvalidOperationException("Repository unexpectedly null. Is event JSON correct?");
        string repoName = webhookEvent.Repository.Name;
        
        services.MockRepoInfo.SetupGet(m => m.Owner).Returns(owner);
        services.MockRepoInfo.SetupGet(m => m.Name).Returns(repoName);
        
        // Set up GitHubClient mock
        string user = webhookEvent.Sender?.Login
            ?? throw new InvalidOperationException("Sender unexpectedly null. Is event JSON correct?");
        services.MockGitHubClient
            .Setup(m => m.IsCollaborator(owner, repoName, user))
            .ReturnsAsync(() => DefaultIsCollaborator);

        return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, services)
            .BuildServiceProvider();
    }

    public string GetCommentBody(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string commentBody = string.Empty;
        switch (eventType)
        {
            case ActionEventType.PullRequestOpened:
                commentBody = (webhookEvent as PullRequestOpenedEvent)?.PullRequest.Body
                    ?? throw new InvalidOperationException();
                
                break;
            case ActionEventType.IssueCommentCreated:
                commentBody = (webhookEvent as IssueCommentCreatedEvent)?.Comment.Body
                    ?? throw new InvalidOperationException();

                break;
            case ActionEventType.IssueCommentEdited:
                commentBody = (webhookEvent as IssueCommentEditedEvent)?.Comment.Body
                    ?? throw new InvalidOperationException();

                break;
        }

        return commentBody;
    }
}