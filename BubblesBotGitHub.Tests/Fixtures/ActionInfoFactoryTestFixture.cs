using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Fixtures;

public sealed class ActionInfoFactoryTestFixture
{
    public readonly string TempDirectory = AssemblyFixture.TestTempDirectory;
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData = AssemblyFixture.TheoryData;

    public HostApplicationBuilder CreateApplicationBuilderWithMocks(WebhookEvent webhookEvent, ActionEventType eventType,
        MockServices serviceMocks) =>
        AssemblyFixture.CreateApplicationBuilderWithMocks(webhookEvent, eventType, serviceMocks);
}