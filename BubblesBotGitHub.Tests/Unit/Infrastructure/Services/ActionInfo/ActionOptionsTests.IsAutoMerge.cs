using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Testing.Platform.Services;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Collection(Collections.Names.ActionOptionsTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.ActionOptionsTests)]
public sealed class IsAutoMerge(ActionOptionsTestFixture classFixture) : IClassFixture<ActionOptionsTestFixture>
{
    [Fact]
    public void ValidInputValue_DoesNotThrow()
    {
        Environment.SetEnvironmentVariable(classFixture.InputAutoMergeEnvName, "true");
        IActionOptions actionOptions = classFixture.BuildActionOptions();

        actionOptions.IsAutoMerge.Should().BeTrue();
    }

    [Fact]
    public void InvalidInputValue_Throws()
    {
        Environment.SetEnvironmentVariable(classFixture.InputAutoMergeEnvName, "gibberish");
        Func<IActionOptions> action = classFixture.BuildActionOptions;

        action.Should().Throw("'auto_merge' must be either 'true' or 'false'");
    }

    [Fact]
    public void NullEnvVar_UsesDefaultValue()
    {
        Environment.SetEnvironmentVariable(classFixture.InputAutoMergeEnvName, null);
        IActionOptions actionOptions = classFixture.BuildActionOptions();
        
        actionOptions.IsAutoMerge.Should().BeFalse();
    }
}