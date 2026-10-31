using System.Text.Json;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using Microsoft.Extensions.Hosting;
using Microsoft.Testing.Platform.Services;
using Moq;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Collection(Collections.Names.PrInfoTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.PrInfoTests)]
public class ConsoleTests(PrInfoTestFixture classFixture) : IClassFixture<PrInfoTestFixture>
{
 [Theory]
 [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
 public async Task InitializeAsyncNullRepository_NullGitHubSummaryPath_PrintsWarning(
     WebhookEvent webhookEvent, ActionEventType eventType)
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
     
     string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
     WebhookEvent eventData = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
         ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
     
     Mock<IGitHubClient> gitHubClientMock = classFixture.CreateGitHubClientMock();
     Mock<IGit> gitMock = new(MockBehavior.Loose);
     IHost host = classFixture.CreateApplicationBuilderWithMocks(
         eventData,
         eventType,
         new MockServices
         {
             MockGit = gitMock,
             MockGitHubClient = gitHubClientMock,
             MockPrInfo = null
         }).Build();
     IPrInfo prInfo = host.Services.GetRequiredService<IPrInfo>();

     await classFixture.ValidateConsoleOutput(prInfo);
     
     // Set original summary path
     Environment.SetEnvironmentVariable("GITHUB_SUMMARY_PATH", originalPath);
 }

 [Theory]
 [MemberData(nameof(PrInfoTestFixture.TheoryData), MemberType = typeof(PrInfoTestFixture))]
 public async Task InitializeAsyncNullRepository_ValidGitHubSummaryPath_AppendsWarning(
     WebhookEvent webhookEvent, ActionEventType eventType)
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
     
     string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
     WebhookEvent eventData = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
         ?? throw new InvalidOperationException($"Cannot deserialize event {json}");

     Mock<IGitHubClient> gitHubClientMock = classFixture.CreateGitHubClientMock();
     Mock<IGit> gitMock = new(MockBehavior.Loose);
     IHost host = classFixture.CreateApplicationBuilderWithMocks(
         eventData,
         eventType,
         new MockServices
         {
             MockGit = gitMock,
             MockGitHubClient = gitHubClientMock,
             MockPrInfo = null
         }).Build();
     IPrInfo prInfo = host.Services.GetRequiredService<IPrInfo>();

     await classFixture.ValidateGitHubSummaryOutput(prInfo);
     
     // Reset summary path
     if (wasNull)
         Environment.SetEnvironmentVariable("GITHUB_SUMMARY_PATH", null);
 }
}