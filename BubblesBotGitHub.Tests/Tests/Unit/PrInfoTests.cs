using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Fixtures;
using BubblesBotGitHub.Tests.Fixtures.PrInfoTests;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Moq;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Tests.Unit;

[UsedImplicitly]
public sealed class PrInfoTests(PrInfoFixture classFixture) : IClassFixture<PrInfoFixture>
{
    private readonly PrInfoFixture _classFixture = classFixture;

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetBaseRefSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().BaseRef, (await subjectFactory()).BaseRef);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetBaseShaSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().BaseSha, (await subjectFactory()).BaseSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadRefSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadRef, (await subjectFactory()).HeadRef);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadShaSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadSha, (await subjectFactory()).HeadSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadLabelSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadLabel, (await subjectFactory()).HeadLabel);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetMergeBaseShaSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().MergeBaseSha, (await subjectFactory()).MergeBaseSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetMergeBaseParentsAmountSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().MergeBaseParentsAmount, (await subjectFactory()).MergeBaseParentsAmount);
    }

    public class InitializeAsync
    {
        [Theory]
        [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
        public async Task ValidEvent_DoesNotThrow(Func<IPrInfo> _, Func<Task<IPrInfo>> subjectFactory)
        {
            Func<Task> action = async () => await subjectFactory();

            await action.Should().NotThrowAsync();
        }
        
        [Fact]
        public async Task InvalidPullRequestOpenedEvent_Throws()
        {
            Mock<IGit> git = new();
            Mock<IGitHubClient> gitHubClient = new();
            WebhookEvent webhookEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
            
            Func<Task> action = async () => await new PrInfo(new Mock<IOptions<Config>>().Object)
                .InitializeAsync(git.Object, gitHubClient.Object, webhookEvent, ActionEventType.PullRequestOpened);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task InvalidIssueCommentEvent_Throws()
        {
            WebhookEvent webhookEvent = AssemblyFixture.GetPullRequestOpenedEvent();
            Func<Task> action = async () => await new PrInfo(new Mock<IOptions<Config>>().Object)
                .InitializeAsync(
                    new Mock<IGit>().Object,
                    new Mock<IGitHubClient>().Object,
                    webhookEvent,
                    ActionEventType.IssueCommentCreated);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
    
    [Collection("Console")]
    public class ConsoleTests(PrInfoFixture classFixture) : IClassFixture<PrInfoFixture>
    {
        [Theory]
        [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
        public async Task InitializeAsyncNullRepository_NullGitHubSummaryPath_PrintsWarning(
            Func<IPrInfo> _, Func<Task<IPrInfo>> subjectFactory)
        {
            string? originalPath = null;
            
            // Check if we're in a GitHub environment
            if (classFixture.GitHubSummaryPath is not null)
            {
                // We're in a GitHub environment, so nullify summary path
                originalPath = classFixture.GitHubSummaryPath;
                Environment.SetEnvironmentVariable("GITHUB_SUMMARY_PATH", null);
            }
            
            JsonSerializerOptions opts = classFixture
                .CreateJsonSerializerOptionsWithRemovedField(typeof(WebhookEvent), "repository");

            WebhookEvent webhookEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
            ActionEventType eventType = ActionEventType.IssueCommentCreated;
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent eventData = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");

            await classFixture.ValidateConsoleOutput(subjectFactory, eventData, eventType);
            
            // Set original summary path
            Environment.SetEnvironmentVariable("GITHUB_SUMMARY_PATH", originalPath);
        }

        [Theory]
        [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
        public async Task InitializeAsyncNullRepository_ValidGitHubSummaryPath_AppendsWarning(Func<IPrInfo> _,
            Func<Task<IPrInfo>> subjectFactory)
        {
            bool wasNull = false;
            
            // If we're not in a GitHub environment, we need to fake a summary path
            if (classFixture.GitHubSummaryPath is null)
            {
                wasNull = true; 
                Environment.SetEnvironmentVariable("GITHUB_SUMMARY_PATH", Path.GetTempFileName());
            }
            
            JsonSerializerOptions opts =
                classFixture.CreateJsonSerializerOptionsWithRemovedField(typeof(WebhookEvent), "repository");

            WebhookEvent webhookEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
            ActionEventType eventType = ActionEventType.IssueCommentCreated;
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent eventData = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");

            await classFixture.ValidateGitHubSummaryOutput(subjectFactory, eventData, eventType);
            
            // Reset summary path
            if (wasNull)
                Environment.SetEnvironmentVariable("GITHUB_SUMMARY_PATH", null);
        }
    }
}