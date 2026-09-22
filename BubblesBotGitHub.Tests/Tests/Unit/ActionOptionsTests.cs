using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Implements;
using BubblesBotGitHub.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace BubblesBotGitHub.Tests.Tests;

public sealed class ActionOptionsTests(ActionOptionsFixture classFixture) : IClassFixture<ActionOptionsFixture>
{
    [Fact]
    public void SuccessfullySetsIsAutoMerge()
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(classFixture.WebhookEvent, classFixture.EventType)
            .BuildServiceProvider();

        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.AutoMergeValue, actionOptions.IsAutoMerge);
    }

    [Fact]
    public void SuccessfullySetsCustomCommand()
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(classFixture.WebhookEvent, classFixture.EventType)
            .BuildServiceProvider();
        
        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.CustomCommandValue, actionOptions.CustomCommand);
    }

    [Fact]
    public void SuccessfullySetsPostComment()
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(classFixture.WebhookEvent, classFixture.EventType)
            .BuildServiceProvider();
        
        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.PostCommentValue, actionOptions.PostComment);
    }
}