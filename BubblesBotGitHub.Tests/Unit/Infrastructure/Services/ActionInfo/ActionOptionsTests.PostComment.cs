using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Collection(Collections.Names.ActionOptionsTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.ActionOptionsTests)]
public sealed class PostComment(ActionOptionsTestFixture classFixture) : IClassFixture<ActionOptionsTestFixture>
{
    [Theory]
    [MemberData(nameof(ActionOptionsTestFixture.PostCommentInputValuesTheoryData), MemberType = typeof(ActionOptionsTestFixture))]
    public void ValidInputValue_DoesNotThrow(string inputValue)
    {
        Environment.SetEnvironmentVariable(classFixture.PostCommentEnvName, inputValue);
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            classFixture.DummyWebhookEvent,
            classFixture.DummyEventType,
            new MockServices
            {
                MockActionOptions = null
            }).Build();

        IActionOptions actionOptions = host.Services.GetRequiredService<IActionOptions>();

        actionOptions.PostComment.Should().Be(inputValue);
    }

    [Fact]
    public void InvalidInputValue_Throws()
    {
        Environment.SetEnvironmentVariable(classFixture.PostCommentEnvName, "invalid-value");
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            classFixture.DummyWebhookEvent,
            classFixture.DummyEventType,
            new MockServices
            {
                MockActionOptions = null
            }).Build();

        Func<IActionOptions> action = () => host.Services.GetRequiredService<IActionOptions>();

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NullEnvVar_UsesDefaultValue()
    {
        Environment.SetEnvironmentVariable(classFixture.PostCommentEnvName, null);
        IActionOptions actionOptions = classFixture.BuildActionOptions();

        actionOptions.PostComment.Should().Be("on-error");
    }
}