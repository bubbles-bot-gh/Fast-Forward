using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public sealed class ActionInfoFactoryTests(ActionInfoFactoryFixture classFixture) : IClassFixture<ActionInfoFactoryFixture>
{
    [Theory]
    [MemberData(nameof(ActionInfoFactoryFixture.TheoryData), MemberType = typeof(ActionInfoFactoryFixture))]
    public void Create_ValidArgs_ReturnsCreatedActionInfo(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IActionInfo actual = classFixture
            .CreateServiceCollectionWithMocks(webhookEvent, eventType)
            .BuildServiceProvider()
            .GetRequiredService<IActionInfo>();

        actual.Should().NotBeNull();
    }
}