using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;


[Trait(Traits.Names.Category, Traits.Categories.Names.ActionInfoFactoryTests)]
public sealed class ActionInfoFactoryTests(ActionInfoFactoryTestFixture classTestFixture) : IClassFixture<ActionInfoFactoryTestFixture>
{
    [Theory]
    [MemberData(nameof(ActionInfoFactoryTestFixture.TheoryData), MemberType = typeof(ActionInfoFactoryTestFixture))]
    public void Create_ValidArgs_ReturnsCreatedActionInfo(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        
        IHost host = classTestFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices
            {
                MockPrInfo = new(MockBehavior.Loose),
                MockActionInfo = null,
                MockActionInfoFactory = null
            }).Build();
        IActionInfo actionInfo = host.Services.GetRequiredService<IActionInfo>();

        actionInfo.Should().NotBeNull();
    }
}