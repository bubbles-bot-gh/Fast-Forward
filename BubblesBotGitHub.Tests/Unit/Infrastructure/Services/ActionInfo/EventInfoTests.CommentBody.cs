using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.BranchProtectionRule;
using Octokit.Webhooks.Models;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Trait(Traits.Names.Category, Traits.Categories.Names.EventInfoTests)]
public sealed class CommentBody(EventInfoTestFixture classFixture) : IClassFixture<EventInfoTestFixture>
{
    [Theory]
    [MemberData(nameof(EventInfoTestFixture.TheoryData), MemberType = typeof(EventInfoTestFixture))]
    public void ValidCommentBody_DoesNotThrow(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string body = classFixture.GetCommentBody(webhookEvent, eventType);
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            classFixture.DefaultMocks()).Build();

        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();

        eventInfo.CommentBody.Should().Be(body);
    }

    [Fact]
    public void InvalidWebhookEvent_ReturnsEmptyString()
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

        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            ActionEventType.PullRequestOpened,
            classFixture.DefaultMocks()).Build();

        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();

        eventInfo.CommentBody.Should().BeEmpty();
    }
}