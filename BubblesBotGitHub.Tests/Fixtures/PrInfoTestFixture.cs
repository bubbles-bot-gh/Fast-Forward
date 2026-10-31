using System.Text.Json;
using System.Text.Json.Serialization;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Octokit;
using Octokit.Internal;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.IGitHubClient;

namespace BubblesBotGitHub.Tests.Fixtures;

[UsedImplicitly]
public class PrInfoTestFixture
{
    public string? GitHubSummaryPath => Environment.GetEnvironmentVariable("GITHUB_SUMMARY_PATH");
    public readonly PullRequestOpenedEvent PullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    public readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    public readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();
    
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData => AssemblyFixture.TheoryData;

    public readonly Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder>
        CreateApplicationBuilderWithMocks = AssemblyFixture.CreateApplicationBuilderWithMocks;

    public Mock<IGitHubClient> CreateGitHubClientMock()
    {
        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        gitHubClientMock.Setup(m => 
                m.GetPullRequestAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(GetDummyPullRequest());

        return gitHubClientMock;
    }

    public PullRequest GetDummyPullRequest()
    {
        string serialized = JsonSerializer.Serialize(PullRequestOpenedEvent.PullRequest, _options);

        return new SimpleJsonSerializer().Deserialize<PullRequest>(serialized);
    }

    public async Task<IPrInfo> BuildPrInfo(WebhookEvent webhookEvent, ActionEventType eventType, Mock<IGit>? gitMock = null)
    {
        
        Mock<IGitHubClient> gitHubClientMock = CreateGitHubClientMock();
        gitMock ??= new Mock<IGit>(MockBehavior.Loose);
            
        IHost host = CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices
            {
                MockGit = gitMock,
                MockGitHubClient = gitHubClientMock,
                MockPrInfo = null
            }).Build();
        IPrInfo prInfo = host.Services.GetRequiredService<IPrInfo>();
        
        await prInfo.InitializeAsync(gitMock.Object, gitHubClientMock.Object);

        return prInfo;
    }

    public readonly Func<Type, string, JsonSerializerOptions> CreateJsonSerializerOptionsWithRemovedField =
        AssemblyFixture.CreateJsonSerializerOptionsWithRemovedField;

    public async Task ValidateConsoleOutput(IPrInfo prInfo)
    {
        TextWriter originalOut = Console.Out;
        await using StringWriter writer = new();
        Console.SetOut(writer);
        await prInfo.InitializeAsync(new Mock<IGit>().Object, new Mock<IGitHubClient>().Object);
    
        string output = writer.ToString();
        output.Should().ContainAll("Repository is null", "GITHUB_SUMMARY_PATH");
    
        // Reset Console.Out for other tests
        Console.SetOut(originalOut);
    }

    public async Task ValidateGitHubSummaryOutput(
        IPrInfo prInfo)
    {
        await prInfo.InitializeAsync(new Mock<IGit>().Object, new Mock<IGitHubClient>().Object);
        StreamReader stream = File.OpenText(GitHubSummaryPath);
        string text = await stream.ReadToEndAsync(TestContext.Current.CancellationToken);
    
        text.Should().Contain("Repository is null");
        text.Should().NotContain("GITHUB_SUMMARY_PATH");
    }
}