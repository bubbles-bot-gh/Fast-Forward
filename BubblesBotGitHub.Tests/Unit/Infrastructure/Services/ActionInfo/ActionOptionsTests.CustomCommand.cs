using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Collection(Collections.Names.ActionOptionsTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.ActionOptionsTests)]
public sealed class CustomCommand(ActionOptionsTestFixture classFixture) : IClassFixture<ActionOptionsTestFixture>
{
    [Fact]
    public void ValidInputValue_DoesNotThrow()
    {
        string inputValue = "/some-custom-command$2892%";
        Environment.SetEnvironmentVariable(classFixture.CustomCommandEnvName, inputValue);
        IActionOptions actionOptions = classFixture.BuildActionOptions();

        actionOptions.CustomCommand.Should().Be(inputValue);
    }

    [Fact]
    public void NullEnvVar_UsesDefaultValue()
    {
        Environment.SetEnvironmentVariable(classFixture.CustomCommandEnvName, null);
        IActionOptions actionOptions = classFixture.BuildActionOptions();

        actionOptions.CustomCommand.Should().Be("/fast-forward");
    }
}