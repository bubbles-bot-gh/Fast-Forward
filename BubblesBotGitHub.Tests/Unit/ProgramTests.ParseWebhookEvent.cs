using BubblesBotGitHub.FastForward;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Unit;

[Collection(Collections.Names.ProgramTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.ProgramTests)]
public sealed class ParseWebhookEvent(ProgramTestFixture classFixture) : IClassFixture<ProgramTestFixture>
{
    [Theory]
    [MemberData(nameof(ProgramTestFixture.TheoryDataBuildHost), MemberType = typeof(ProgramTestFixture))]
    public async Task ValidEvent_SuccessfullyParses(string eventPath, string eventName, Type expectedWebhookType, ActionEventType expectedEventType)
    {
        ProgramTestFixture.SetEnvVars(eventPath, eventName);
        (WebhookEvent webhookEvent, ActionEventType eventType) = await Program.ParseWebhookEvent();

        webhookEvent.Should().BeOfType(expectedWebhookType);
        eventType.Should().Be(expectedEventType);
    }
}