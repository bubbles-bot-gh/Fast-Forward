using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Fixtures;
using BubblesBotGitHub.Tests.Fixtures.PrInfoTests;
using FluentAssertions;
using JetBrains.Annotations;
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

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task InitializeAsync_InvalidPullRequestOpenedEvent_Throws(
        Func<IPrInfo> _, Func<Task<IPrInfo>> subjectFactory)
    {
        Mock<IGit> git = new();
        Mock<IGitHubClient> gitHubClient = new();
        WebhookEvent webhookEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
            
        Func<Task> action = async () => await (await subjectFactory())
            .InitializeAsync(git.Object, gitHubClient.Object, webhookEvent, ActionEventType.PullRequestOpened);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task InitializeAsync_InvalidIssueCommentCreatedEvent_Throws(
        Func<IPrInfo> _, Func<Task<IPrInfo>> subjectFactory)
    {
        Mock<IGit> git = new();
        Mock<IGitHubClient> gitHubClient = new();
        WebhookEvent webhookEvent = AssemblyFixture.GetIssueCommentEditedEvent();
            
        Func<Task> action = async () => await (await subjectFactory())
            .InitializeAsync(git.Object, gitHubClient.Object, webhookEvent, ActionEventType.IssueCommentCreated);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task InitializeAsync_InvalidIssueCommentEditedEvent_Throws(
        Func<IPrInfo> _, Func<Task<IPrInfo>> subjectFactory)
    {
        Mock<IGit> git = new();
        Mock<IGitHubClient> gitHubClient = new();
        WebhookEvent webhookEvent = AssemblyFixture.GetPullRequestOpenedEvent();
            
        Func<Task> action = async () => await (await subjectFactory())
            .InitializeAsync(git.Object, gitHubClient.Object, webhookEvent, ActionEventType.IssueCommentEdited);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Collection("Console")]
    public class ConsoleTests
    {
        [Theory]
        [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
        public async Task InitializeAsync_NullRepository_PrintsWarning(Func<IPrInfo> _, Func<Task<IPrInfo>> subjectFactory)
        {
            Mock<IGit> git = new();
            Mock<IGitHubClient> gitHubClient = new();

            JsonSerializerOptions opts = new()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                {
                    Modifiers =
                    {
                        typeInfo =>
                        {
                            if (typeof(WebhookEvent).IsAssignableFrom(typeInfo.Type))
                            {
                                // Remove property from deserialization contract, skipping "repository" key
                                JsonPropertyInfo? repo = typeInfo
                                    .Properties
                                    .FirstOrDefault(p => p.Name == "repository");

                                if (repo is not null)
                                {
                                    typeInfo.Properties.Remove(repo);
                                }
                            }
                        }
                    }
                }
            };

            WebhookEvent webhookEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent eventData = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");

            string? summaryPath = Environment.GetEnvironmentVariable("GITHUB_SUMMARY_PATH");

            if (summaryPath is null)
            {
                TextWriter originalOut = Console.Out;
                await using StringWriter writer = new();
                Console.SetOut(writer);
                
                await RunInitializeAsync();
                
                string output = writer.ToString();
                output.Should().ContainAll("Repository is null", "GITHUB_SUMMARY_PATH");
                
                Console.SetOut(originalOut);

                return;
            }

            await RunInitializeAsync();
            StreamReader stream = File.OpenText(summaryPath);
            string text = await stream.ReadToEndAsync(TestContext.Current.CancellationToken);

            text.Should().Contain("Repository is null");
            text.Should().NotContain("GITHUB_SUMMARY_PATH");
            return;
            
            async Task RunInitializeAsync()
            {
                await (await subjectFactory())
                    .InitializeAsync(
                        git.Object,
                        gitHubClient.Object,
                        eventData,
                        ActionEventType.IssueCommentCreated);
            }
        }
    }
}