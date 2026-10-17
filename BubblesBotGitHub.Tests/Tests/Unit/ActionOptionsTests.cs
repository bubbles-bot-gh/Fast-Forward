using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public sealed class ActionOptionsTests(ActionOptionsFixture classFixture) : IClassFixture<ActionOptionsFixture>
{
    [Theory] 
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void SuccessfullySetsIsAutoMerge(WebhookEvent webhookEvent)
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, classFixture.EventType)
            .BuildServiceProvider();

        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.AutoMergeValue, actionOptions.IsAutoMerge);
    }

    [Theory] 
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void SuccessfullySetsCustomCommand(WebhookEvent webhookEvent)
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, classFixture.EventType)
            .BuildServiceProvider();
        
        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.CustomCommandValue, actionOptions.CustomCommand);
    }

    [Theory] 
    [MemberData(nameof(ActionOptionsFixture.TheoryData), MemberType = typeof(ActionOptionsFixture))]
    public void SuccessfullySetsPostComment(WebhookEvent webhookEvent)
    {
        IServiceProvider provider = new ServiceCollection()
            .AddAppServices(webhookEvent, classFixture.EventType)
            .BuildServiceProvider();
        
        IActionOptions actionOptions = provider.GetRequiredService<IActionOptions>();
        
        Assert.Equal(classFixture.PostCommentValue, actionOptions.PostComment);
    }
}