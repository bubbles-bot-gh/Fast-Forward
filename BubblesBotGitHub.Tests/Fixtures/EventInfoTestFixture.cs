using System.Text.Json;
using System.Text.Json.Nodes;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

[UsedImplicitly]
public sealed class EventInfoTestFixture
{
    public readonly string CustomCommand = "/fast-forward";
    public readonly string PullRequestOpenedJsonPath = AssemblyFixture.PullRequestOpenedEventPath;
    
    private const bool IsCollaborator = true;
    private const string Name = "Fast-Forward";
    private const string Owner = "bubbles-bot-gh";
    public readonly string User = "luneisolei";
    private static readonly bool CommandInvoked = true;
    private static readonly bool DefaultIsCollaborator = true;
    public readonly PullRequestOpenedEvent PullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    private static readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();
    
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData => AssemblyFixture.TheoryData;

    public readonly Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder>
        CreateApplicationBuilderWithMocks = AssemblyFixture.CreateApplicationBuilderWithMocks;

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
        services.MockActionOptions.Setup(m => m.CustomCommand).Returns(CustomCommand);
        
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
            .Setup(m => m.IsCollaboratorAsync(owner, repoName, user))
            .ReturnsAsync(() => DefaultIsCollaborator);

        return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, services)
            .BuildServiceProvider();
    }

    public Mock<IActionOptions> DefaultActionOptionsMock()
    {
        Mock<IActionOptions> actionOptionsMock = new(MockBehavior.Strict);
        actionOptionsMock.Setup(m => m.CustomCommand).Returns(CustomCommand);

        return actionOptionsMock;
    }

    public Mock<IRepoInfo> DefaultRepoInfoMock()
    {
        Mock<IRepoInfo> repoInfoMock = new(MockBehavior.Strict);
        repoInfoMock.Setup(m => m.Name).Returns(Name);
        repoInfoMock.Setup(m => m.Owner).Returns(Owner);

        return repoInfoMock;
    }

    private Mock<IGitHubClient> DefaultGitHubClientMock()
    {
        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        gitHubClientMock.Setup(m => m.IsCollaboratorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IsCollaborator);

        return gitHubClientMock;
    }

    public MockServices DefaultMocks()
    {
        return new MockServices
        {
            MockActionOptions = DefaultActionOptionsMock(),
            MockRepoInfo = DefaultRepoInfoMock(),
            MockGitHubClient = DefaultGitHubClientMock(),
            MockEventInfo = null
        };
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

    public JsonNode DeserializeWebhookEventWithCustomField(string json, string? value, params string[] path)
    {
        JsonNode root = JsonNode.Parse(json)
            ?? throw new InvalidOperationException("JSON parse result in null. Does file exist?");

        JsonNode current = root;
        for (int i = 0; i < path.Length - 1; i++)
        {
            current = current[path[i]]
                ?? throw new ArgumentException($"Path segment '{path[i]}' not found in JSON.");
        }

        current[path[^1]] = value;

        return root;
    }

    public HostApplicationBuilder SetupCommandInvokedTest(string commentBody)
    {        
        string json = File.ReadAllText(PullRequestOpenedJsonPath);
        JsonNode root = DeserializeWebhookEventWithCustomField(json, commentBody, "pull_request", "body");
        WebhookEvent webhookEvent = JsonSerializer.Deserialize<PullRequestOpenedEvent>(root.ToJsonString())!;
        
        Mock<IActionOptions> actionOptionsMock = new(MockBehavior.Strict);
        actionOptionsMock.Setup(m => m.CustomCommand).Returns(CustomCommand);

        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        gitHubClientMock.Setup(m => m.IsCollaboratorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        return CreateApplicationBuilderWithMocks(
            webhookEvent,
            ActionEventType.PullRequestOpened,
            DefaultMocks());
    }
}