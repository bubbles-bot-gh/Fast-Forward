using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public sealed class ActionOptionsTests(ActionOptionsFixture classFixture) : IClassFixture<ActionOptionsFixture>
{
    [Theory] 
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void SuccessfullySetsIsAutoMerge(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();

        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.AutoMergeValue, actionOptions.IsAutoMerge);
    }

    [Theory] 
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void SuccessfullySetsCustomCommand(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.CustomCommandValue, actionOptions.CustomCommand);
    }

    [Theory] 
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void SuccessfullySetsPostComment(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.PostCommentValue, actionOptions.PostComment);
    }

    [Theory]
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void ThrowsWhenPostCommentValueIsInvalid(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string oldValue = Environment.GetEnvironmentVariable("INPUT_POST_COMMENT") ?? "on-error";
        Environment.SetEnvironmentVariable("INPUT_POST_COMMENT", "some-invalid-value");
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        Action action = () => provider.GetRequiredService<IActionOptions>();
        
        action.Should().Throw<InvalidOperationException>();
        
        Environment.SetEnvironmentVariable("INPUT_POST_COMMENT", oldValue);
    }

    [Theory]
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void PostCommentUsesDefaultValueWhenEnvironmentVariableIsNull(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        string oldValue = Environment.GetEnvironmentVariable("INPUT_POST_COMMENT") ?? "on-error";
        Environment.SetEnvironmentVariable("INPUT_POST_COMMENT", null);
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        string actual = provider.GetRequiredService<IActionOptions>().PostComment;
        
        actual.Should().Be("on-error");
        
        Environment.SetEnvironmentVariable("INPUT_POST_COMMENT", oldValue);
    }

    [Theory]
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void CustomCommandUsesDefaultValueWhenEnvironmentVariableIsNull(WebhookEvent webhookEvent,
        ActionEventType eventType)
    {
        string oldValue = Environment.GetEnvironmentVariable("INPUT_CUSTOM_COMMAND") ?? "/fast-forward";
        Environment.SetEnvironmentVariable("INPUT_CUSTOM_COMMAND", null);
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        string actual = provider.GetRequiredService<IActionOptions>().CustomCommand;
        
        actual.Should().Be("/fast-forward");
        
        Environment.SetEnvironmentVariable("INPUT_CUSTOM_COMMAND", oldValue);
    }

    [Theory]
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void IsAutoMergeUsesDefaultValueWhenEnvironmentVariableIsNull(WebhookEvent webhookEvent,
        ActionEventType eventType)
    {
        string oldValue = Environment.GetEnvironmentVariable("INPUT_AUTO_MERGE") ?? "false";
        Environment.SetEnvironmentVariable("INPUT_AUTO_MERGE", null);
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, eventType)
            .BuildServiceProvider();
        
        bool actual = provider.GetRequiredService<IActionOptions>().IsAutoMerge;
        
        actual.Should().Be(false);
        
        Environment.SetEnvironmentVariable("INPUT_AUTO_MERGE", oldValue);
    }
}