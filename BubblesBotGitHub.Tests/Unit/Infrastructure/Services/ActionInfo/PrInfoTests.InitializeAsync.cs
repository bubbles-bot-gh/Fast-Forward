using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.IGitHubClient;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Collection(Collections.Names.PrInfoTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.PrInfoTests)]
public sealed class InitializeAsync(PrInfoTestFixture classFixture) : IClassFixture<PrInfoTestFixture>
{
    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectBaseRef(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string expected = classFixture.GetDummyPullRequest().Base.Ref;
        IPrInfo prInfo = await classFixture.BuildPrInfo(webhookEvent, eventType);
        
        prInfo.BaseRef.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectBaseSha(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string expected = classFixture.GetDummyPullRequest().Base.Sha;
        IPrInfo prInfo = await classFixture.BuildPrInfo(webhookEvent, eventType);

        prInfo.BaseSha.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectHeadRef(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string expected = classFixture.GetDummyPullRequest().Head.Ref;
        IPrInfo prInfo = await classFixture.BuildPrInfo(webhookEvent, eventType);

        prInfo.HeadRef.Should().Be(expected);
    }
    
    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectHeadSha(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string expected = classFixture.GetDummyPullRequest().Head.Sha;
        IPrInfo prInfo = await classFixture.BuildPrInfo(webhookEvent, eventType);

        prInfo.HeadSha.Should().Be(expected);
    }
    
    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectHeadLabel(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string expected = classFixture.GetDummyPullRequest().Head.Label;
        IPrInfo prInfo = await classFixture.BuildPrInfo(webhookEvent, eventType);

        prInfo.HeadLabel.Should().Be(expected);
    }
    
    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectMergeBaseSha(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string expected = classFixture.GetDummyPullRequest().MergeCommitSha;
        IPrInfo prInfo = await classFixture.BuildPrInfo(webhookEvent, eventType);

        prInfo.MergeBaseSha.Should().Be(expected);
    }
    
    [Theory]
    [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
    public async Task ValidEvent_SetsCorrectParentsCount(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        Mock<IGit> gitMock = new(MockBehavior.Loose);
        IPrInfo _ = await classFixture.BuildPrInfo(webhookEvent, eventType, gitMock);

        gitMock.Verify(m => m.GetParentCount(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task InvalidPullRequestOpenedEvent_Throws()
    {
        WebhookEvent webhookEvent = classFixture.IssueCommentCreatedEvent;
        Func<Task<IPrInfo>> action = () => classFixture.BuildPrInfo(webhookEvent, ActionEventType.PullRequestOpened);

        (await action.Should().ThrowAsync<InvalidOperationException>()).WithMessage($"Could not cast '{typeof(WebhookEvent).FullName}' to "
            + $"'{typeof(PullRequestOpenedEvent).FullName}'.'");
    }

    [Fact]
    public async Task InvalidIssueCommentEvent_Throws()
    {
        WebhookEvent webhookEvent = classFixture.PullRequestOpenedEvent;
        Func<Task<IPrInfo>> action = () => classFixture.BuildPrInfo(webhookEvent, ActionEventType.IssueCommentCreated);

        (await action.Should().ThrowAsync<InvalidOperationException>()).WithMessage($"Could not cast "
            + $"'{typeof(WebhookEvent).FullName}' to '{typeof(IssueCommentCreatedEvent).FullName}'.");
    }
}