using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit.Webhooks;
using EventInfo = BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo.EventInfo;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public sealed class EventInfoTests(ITestOutputHelper outputHelper, EventInfoFixture classFixture) : IClassFixture<EventInfoFixture>
{
    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public void GetCommentBodySucceeds(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        // Set up actual
        IServiceProvider provider = classFixture.CreateServiceProviderWithMocks(webhookEvent, eventType);
        IEventInfo actual = provider.GetRequiredService<IEventInfo>();
        
        // Compare to expected
        string expected = classFixture.GetCommentBody(webhookEvent, eventType);
        actual.CommentBody.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public void CommandInvokedIsTrueWhenCustomCommandMatches(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string commentBody = classFixture.GetCommentBody(webhookEvent, eventType);
        Mock<IActionOptions> optionsMock = new();
        
        // Just make the custom command equate to the comment body text. It's less complicated than changing comment
        // body to match the custom command.
        optionsMock.Setup(m => m.CustomCommand).Returns(commentBody.Trim());
        
        Mock<IRepoInfo> repoInfoMock = new();
        Mock<IGitHubClient> gitHubClientMock = new();
        gitHubClientMock
            .Setup(m => m.IsCollaborator(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        IEventInfo actual = new EventInfo(webhookEvent, optionsMock.Object, repoInfoMock.Object, gitHubClientMock.Object);

        actual.CommandInvoked.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public void CommandInvokedIsFalseWhenCustomCommandDoesNotMatch(WebhookEvent webhookEvent, ActionEventType _)
    {
        Mock<IActionOptions> optionsMock = new();
        optionsMock.Setup(m => m.CustomCommand).Returns("I'm-not-a-real-command");
        
        Mock<IRepoInfo> repoInfoMock = new();
        Mock<IGitHubClient> gitHubClientMock = new();
        gitHubClientMock
            .Setup(m => m.IsCollaborator(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        IEventInfo actual = new EventInfo(webhookEvent, optionsMock.Object, repoInfoMock.Object, gitHubClientMock.Object);
        
        actual.CommandInvoked.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public void GetsCorrectUser(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IServiceProvider provider = classFixture.CreateServiceProviderWithMocks(webhookEvent, eventType);
        IEventInfo actual = provider.GetRequiredService<IEventInfo>();

        actual.User.Should().Be(webhookEvent.Sender?.Login);
    }

    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public async Task GetsCorrectUserHasPermsValue(WebhookEvent webhookEvent, ActionEventType _)
    {
        Task<bool> expected = new Task<bool>(() => true);
        
        Mock<IActionOptions> optionsMock = new();
        optionsMock.Setup(m => m.CustomCommand).Returns("/fast-forward");
        
        Mock<IRepoInfo> repoInfoMock = new();
        Mock<IGitHubClient> gitHubClientMock = new();
        gitHubClientMock
            .Setup(m => m.IsCollaborator(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expected);

        IEventInfo actual = new EventInfo(webhookEvent, optionsMock.Object, repoInfoMock.Object, gitHubClientMock.Object);
        
        actual.UserHasPerms.Should().Be(expected);
    }
}