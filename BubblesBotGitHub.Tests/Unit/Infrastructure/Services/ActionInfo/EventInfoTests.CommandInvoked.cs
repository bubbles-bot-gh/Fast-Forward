using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Trait(Traits.Names.Category, Traits.Categories.Names.EventInfoTests)]
public sealed class CommandInvoked(EventInfoTestFixture classFixture) : IClassFixture<EventInfoTestFixture>
{
    [Fact]
    public void Match_ReturnsTrue()
    {
        IHost host = classFixture.SetupCommandInvokedTest(classFixture.CustomCommand).Build();
        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();

        eventInfo.CommandInvoked.Should().BeTrue();
    }

    [Fact]
    public void NoMatch_ReturnsFalse()
    {
        IHost host = classFixture.SetupCommandInvokedTest(string.Empty).Build();
        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();

        eventInfo.CommandInvoked.Should().BeFalse();
    }
}