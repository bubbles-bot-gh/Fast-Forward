using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.BranchProtectionRule;
using Octokit.Webhooks.Models;
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

    [Fact]
    public void CommentBody_InvalidWebhookEvent_ReturnsEmptyString()
    {
        BranchProtectionRuleEvent webhookEvent = new BranchProtectionRuleCreatedEvent
        {
            Rule = new BranchProtectionRule
            {
                Name = "New Branch Protection Rule A",
                AllowDeletionsEnforcementLevel = EnforcementLevel.NonAdmins,
                AllowForcePushesEnforcementLevel = EnforcementLevel.Everyone,
                AuthorizedActorNames = [],
                LinearHistoryRequirementEnforcementLevel = EnforcementLevel.Everyone,
                MergeQueueEnforcementLevel = EnforcementLevel.Everyone,
                PullRequestReviewsEnforcementLevel = EnforcementLevel.Everyone,
                RequiredConversationResolutionLevel = EnforcementLevel.Everyone,
                RequiredDeploymentsEnforcementLevel = EnforcementLevel.Everyone,
                RequiredStatusChecks = [],
                RequiredStatusChecksEnforcementLevel = EnforcementLevel.Everyone,
                SignatureRequirementEnforcementLevel = EnforcementLevel.Everyone
            }
        };
        IServiceProvider provider = classFixture.CreateServiceProviderWithMocks(
            webhookEvent, 
            ActionEventType.PullRequestOpened, 
            new MockServices { MockEventInfo = null });
        IEventInfo actual = provider.GetRequiredService<IEventInfo>();

        actual.CommentBody.Should().Be(string.Empty);
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