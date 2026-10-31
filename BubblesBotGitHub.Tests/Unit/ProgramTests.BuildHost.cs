using BubblesBotGitHub.FastForward;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Unit;

[Collection(Collections.Names.ProgramTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.ProgramTests)]
public sealed class BuildHost(ProgramTestFixture classFixture) : IClassFixture<ProgramTestFixture>
{
    [Theory]
    [MemberData(nameof(ProgramTestFixture.TheoryDataBuildHost), MemberType = typeof(ProgramTestFixture))]
    public async Task ValidEnvVars_BuildsHost(string eventPath, string eventName, Type _1, ActionEventType _2)
    {
        (WebhookEvent webhookEvent, ActionEventType eventType) = await ProgramTestFixture.ParseWebhookEvent(eventPath, eventName);
        Func<IHost> action = () => Program.BuildHost(webhookEvent, eventType);

        action.Should().NotThrow().Which.Should().NotBeNull();
    }
}