using System.Text.Json;
using System.Text.Json.Nodes;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Trait(Traits.Names.Category, Traits.Categories.Names.EventInfoTests)]
public sealed class User(EventInfoTestFixture classFixture) : IClassFixture<EventInfoTestFixture>
{
    [Fact] // User should be available on the generic WebhookEvent
    public void ValidUser_ReturnsUser()
    {
        string json = File.ReadAllText(classFixture.PullRequestOpenedJsonPath);
        JsonNode root = classFixture.DeserializeWebhookEventWithCustomField(json, classFixture.User, "sender", "login");
        WebhookEvent webhookEvent = JsonSerializer.Deserialize<PullRequestOpenedEvent>(root.ToJsonString())!;

        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            ActionEventType.PullRequestOpened,
            classFixture.DefaultMocks()).Build();
        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();

        eventInfo.User.Should().Be(classFixture.User);
    }

    [Fact]
    public void MissingUser_ReturnsEmpty()
    {
        string json = File.ReadAllText(classFixture.PullRequestOpenedJsonPath);
        JsonNode root = classFixture.DeserializeWebhookEventWithCustomField(json, null, "sender");
        WebhookEvent webhookEvent = JsonSerializer.Deserialize<PullRequestOpenedEvent>(root.ToJsonString())!;

        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            ActionEventType.PullRequestOpened,
            classFixture.DefaultMocks()).Build();
        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();

        eventInfo.User.Should().Be(string.Empty);
    }
}