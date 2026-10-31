using System.Text.Json;
using BubblesBotGitHub.FastForward;
using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

public class ProgramTestFixture
{
    private const string EventPathEnvName = "GITHUB_EVENT_PATH";
    private const string EventNameEnvName = "GITHUB_EVENT_NAME";
    private const string TestIdTokenToken = "some-test-id-token";
    private const string TestIdTokenUrl = "http://localhost:8080";
    private static readonly string ActionsIdTokenRequestTokenEnvName = AssemblyFixture.IdTokenRequestTokenEnvName;
    private static readonly string ActionsIdTokenRequestUrlEnvName = AssemblyFixture.IdTokenRequestTokenUrlEnvName;
    private const bool CommandInvokedDefault = false;
    private const bool IsAutoMergeDefault = false;
    private const string PostCommentDefault = "always";
    private static readonly string PullRequestOpenedEventPath = AssemblyFixture.PullRequestOpenedEventPath;
    private static readonly string IssueCommentCreatedEventPath = AssemblyFixture.IssueCommentCreatedEventPath;
    private static readonly string IssueCommentEditedEventPath = AssemblyFixture.IssueCommentEditedEventPath;
    private static readonly string PullRequestEventName = "pull_request";
    private static readonly string IssueCommentEventName = "issue_comment";
    
    public static TheoryData<string, string, Type, ActionEventType> TheoryDataBuildHost =>
    [
        new TheoryDataRow<string, string, Type, ActionEventType>(
            PullRequestOpenedEventPath, 
            PullRequestEventName, 
            typeof(PullRequestOpenedEvent), 
            ActionEventType.PullRequestOpened),
        new TheoryDataRow<string, string, Type, ActionEventType>(
            IssueCommentCreatedEventPath, 
            IssueCommentEventName, 
            typeof(IssueCommentCreatedEvent), 
            ActionEventType.IssueCommentCreated),
        new TheoryDataRow<string, string, Type, ActionEventType>(
            IssueCommentEditedEventPath, 
            IssueCommentEventName, 
            typeof(IssueCommentEditedEvent), 
            ActionEventType.IssueCommentEdited)
    ];

    private static TheoryData<string, string, bool,  bool, bool, string, Times, Times, Times, int> BuildTheoryDataRows(ActionEventType eventType,
        string eventPath, string eventName) =>
    [
        new(eventPath, eventName, true, true, false,
            PostComment.Always.ToStringValue(), Times.Once(), Times.Once(), Times.Once(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: true, shouldExit: false, postComment: 'always'"
        },
        
        new(eventPath, eventName, true, true, false,
            PostComment.Never.ToStringValue(), Times.Once(), Times.Once(), Times.Never(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: true, shouldExit: false, postComment: 'never'"
        },
        
        new(eventPath, eventName, true, true, false,
            PostComment.OnError.ToStringValue(), Times.Once(), Times.Once(), Times.Never(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: true, shouldExit: false, postComment: 'on-error' + no error"
        },
        
        new(eventPath, eventName, true, false, false,
            PostComment.Always.ToStringValue(), Times.Once(), 
            eventType != ActionEventType.PullRequestOpened ? Times.Once() : Times.Never(), Times.Once(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: false, shouldExit: false, postComment: always"
        },
        
        new(eventPath, eventName, true, false, false,
            PostComment.Never.ToStringValue(), Times.Once(), 
            eventType != ActionEventType.PullRequestOpened ? Times.Once() : Times.Never(), Times.Never(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: false, shouldExit: false, postComment: never"
        },
        
        new(eventPath, eventName, true, false, false,
            PostComment.OnError.ToStringValue(), Times.Once(), 
            eventType != ActionEventType.PullRequestOpened ? Times.Once() : Times.Never(), Times.Never(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: false, shouldExit: false, postComment: on-error + no error"
        },
        
        new(eventPath, eventName, false, false, false,
            PostComment.Never.ToStringValue(), Times.Never(), Times.Never(), Times.Never(), 0)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: false, isAutoMerge: false, shouldExit: false, postComment: never"
        },
        
        new(eventPath, eventName, false, true, false,
        PostComment.Always.ToStringValue(), Times.Never(), Times.Never(), Times.Never(), 0)
        {
        Label = $"eventType: {eventType.ToString()}, commandInvoked: false, isAutoMerge: true, shouldExit: false, postComment: always"
        },
        
        new(eventPath, eventName, true, true, true,
            PostComment.Always.ToStringValue(), Times.Once(), Times.Never(), Times.Once(), 1)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: true, shouldExit: true, postComment: always"
        },
        
        new(eventPath, eventName, true, true, true,
            PostComment.Never.ToStringValue(), Times.Once(), Times.Never(), Times.Never(), 1)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: true, shouldExit: true, postComment: never"
        },
        
        new(eventPath, eventName, true, true, true,
            PostComment.OnError.ToStringValue(), Times.Once(), Times.Never(), Times.Once(), 1)
        {
            Label = $"eventType: {eventType.ToString()}, commandInvoked: true, isAutoMerge: true, shouldExit: true, postComment: on-error"
        }
    ];

    public static TheoryData<string, string, bool,  bool, bool, string, Times, Times, Times, int> TheoryDataFastForwardRunner() =>
    [
        ..BuildTheoryDataRows(ActionEventType.PullRequestOpened, PullRequestOpenedEventPath, PullRequestEventName),
        ..BuildTheoryDataRows(ActionEventType.IssueCommentCreated, IssueCommentCreatedEventPath, IssueCommentEventName),
        ..BuildTheoryDataRows(ActionEventType.IssueCommentEdited, IssueCommentEditedEventPath, IssueCommentEventName)
    ];

    private static Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder> CreateApplicationBuilderWithMocks =>
        AssemblyFixture.CreateApplicationBuilderWithMocks;

    public static void SetEnvVars(string eventPath, string eventName)
    {
        Environment.SetEnvironmentVariable(EventPathEnvName, eventPath);
        Environment.SetEnvironmentVariable(EventNameEnvName, eventName);
        Environment.SetEnvironmentVariable(ActionsIdTokenRequestTokenEnvName, TestIdTokenToken);
        Environment.SetEnvironmentVariable(ActionsIdTokenRequestUrlEnvName, TestIdTokenUrl);
    }

    public static async Task<(WebhookEvent, ActionEventType)> ParseWebhookEvent(string eventPath, string eventName)
    {
        JsonDocument json = JsonDocument.Parse(await File.ReadAllTextAsync(eventPath));
        
        // Read the "action" field to fill out our specific event name
        string? action = null;
        if(json.RootElement.TryGetProperty("action", out JsonElement a))
            action = a.GetString();
        string specificEvent = action is null ? eventName : $"{eventName}_{action}";
        
        // Use the specific event to return both a WebhookEvent and ActionEventType
        return specificEvent switch
        {
            "pull_request_opened" => (DeserializeWebhookEvent<PullRequestOpenedEvent>(json), 
                ActionEventType.PullRequestOpened),
            "issue_comment_edited" => (DeserializeWebhookEvent<IssueCommentEditedEvent>(json), 
                ActionEventType.IssueCommentEdited),
            "issue_comment_created" => (DeserializeWebhookEvent<IssueCommentCreatedEvent>(json),
                ActionEventType.IssueCommentCreated),
            _ => throw new NotSupportedException($"Unsupported event type: {specificEvent}")
        };
        
        TWebhookEvent DeserializeWebhookEvent<TWebhookEvent>(JsonDocument jsonString)
            where TWebhookEvent : WebhookEvent =>
            jsonString.Deserialize<TWebhookEvent>()
            ?? throw new JsonException($"Failed to deserialize webhook event as {typeof(TWebhookEvent).Name}");
    }

    private static Mock<IGitHubClientFactory> CreateGitHubClientFactoryMock()
    {
        Mock<IGitHubClientFactory> gitHubClientFactoryMock = new();
        gitHubClientFactoryMock.Setup(m => m.Create()).Returns(new Mock<IGitHubClient>().Object);

        return gitHubClientFactoryMock;
    }

    private static Mock<IActionInfo> CreateActionInfoMock(
        bool commandInvoked = CommandInvokedDefault, 
        bool isAutoMerge = IsAutoMergeDefault,
        string postComment = PostCommentDefault,
        bool shouldExit = false)
    {
        Mock<IActionInfo> actionInfoMock = new(MockBehavior.Loose);
        Mock<IEventInfo> eventInfoMock = CreateEventInfoMock(commandInvoked, shouldExit);
        Mock<IActionOptions> actionOptionsMock = CreateActionOptionsMock(isAutoMerge, postComment);
        Mock<IRepoInfo> repoInfoMock = CreateRepoInfoMock();
        Mock<IPrInfo> prInfoMock = CreatePrInfoMock();
        
        actionInfoMock.Setup(m => m.EventInfo).Returns(eventInfoMock.Object);
        actionInfoMock.Setup(m => m.ActionOptions).Returns(actionOptionsMock.Object);
        actionInfoMock.Setup(m => m.PrInfo).Returns(prInfoMock.Object);
        actionInfoMock.Setup(m => m.RepoInfo).Returns(repoInfoMock.Object);

        return actionInfoMock;
    }

    private static Mock<IActionOptions> CreateActionOptionsMock(
        bool isAutoMerge = IsAutoMergeDefault, string postComment = PostCommentDefault)
    {
        Mock<IActionOptions> actionOptionsMock = new(MockBehavior.Loose);
        actionOptionsMock.SetupGet(m => m.IsAutoMerge).Returns(isAutoMerge);
        actionOptionsMock.SetupGet(m => m.PostComment).Returns(postComment);

        return actionOptionsMock;
    }

    private static Mock<IEventInfo> CreateEventInfoMock(bool commandInvoked = CommandInvokedDefault, bool shouldExit = false)
    {
        Mock<IEventInfo> eventInfoMock = new(MockBehavior.Loose);
        eventInfoMock.SetupGet(m => m.CommandInvoked).Returns(commandInvoked);
        eventInfoMock.SetupGet(m => m.ShouldExit).Returns(shouldExit);
        
        return eventInfoMock;
    }

    private static Mock<IRepoInfo> CreateRepoInfoMock()
    {
        Mock<IRepoInfo> repoInfoMock = new(MockBehavior.Loose);

        return repoInfoMock;
    }

    private static Mock<IPrInfo> CreatePrInfoMock()
    {
        Mock<IPrInfo> prInfoMock = new(MockBehavior.Loose);

        return prInfoMock;
    }

    public async Task<IHost> BuildHost(
        string eventPath, 
        string eventName, 
        bool commandInvoked = CommandInvokedDefault, 
        bool isAutoMerge = IsAutoMergeDefault,
        string postComment = PostCommentDefault,
        bool shouldExit = false)
    {
        SetEnvVars(eventPath, eventName);
        (WebhookEvent webhookEvent, ActionEventType eventType) = await ParseWebhookEvent(eventPath, eventName);
        
        return CreateApplicationBuilderWithMocks(webhookEvent, eventType, new MockServices
        {
            MockGitHubClientFactory = CreateGitHubClientFactoryMock(),
            MockActionInfo = CreateActionInfoMock(commandInvoked, isAutoMerge, postComment, shouldExit)
        }).Build();
    }

    public static void VerifyCommentBuildCalled(IHost host, Times? times = null)
    {
        Times callTimes = times ?? Times.Once();
        Mock<ICommentBuilder> commentBuilderMock = Mock.Get(host.Services.GetRequiredService<ICommentBuilder>());
        commentBuilderMock.Verify(m => m.Build(), callTimes);
    }

    public static void VerifyFastForwardAsyncCalled(IHost host, Times? times = null)
    {
        Times callTimes = times ?? Times.Once();
        Mock<IGitHubClient> gitHubClientMock = Mock.Get(host.Services.GetRequiredService<IGitHubClient>());
        IActionInfo actionInfo = Mock.Get(host.Services.GetRequiredService<IActionInfo>()).Object;
        
        gitHubClientMock.Verify(m => m.FastForwardAsync(
            actionInfo.RepoInfo.Owner,
            actionInfo.RepoInfo.Name,
            actionInfo.PrInfo.BaseLabel,
            actionInfo.PrInfo.HeadSha), callTimes);
    }

    public static void VerifyPostCommentAsyncCalled(IHost host, Times? times = null)
    {
        Times callTimes = times ?? Times.Once();
        Mock<IGitHubClient> gitHubClientMock = Mock.Get(host.Services.GetRequiredService<IGitHubClient>());
        IActionInfo actionInfo = Mock.Get(host.Services.GetRequiredService<IActionInfo>()).Object;
        
        gitHubClientMock.Verify(m => m.PostCommentAsync(
            actionInfo.RepoInfo.Owner, 
            actionInfo.RepoInfo.Name,
            (uint)actionInfo.PrInfo.IssueNumber,
            It.IsAny<string>()), callTimes);
    }
}