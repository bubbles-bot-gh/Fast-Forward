using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Implements;
using BubblesBotGitHub.Tests.Unit.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace BubblesBotGitHub.Tests.Unit.Tests;

public sealed class ActionOptionsTests : IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable(
            ActionOptionsFixture.AutoMergeEnvName, 
            AssemblyFixture.AutoMerge.ToString());
        
        Environment.SetEnvironmentVariable(
            ActionOptionsFixture.CustomCommandEnvName, 
            AssemblyFixture.CustomCommand);
        
        Environment.SetEnvironmentVariable(
            ActionOptionsFixture.PostCommentEnvName, 
            AssemblyFixture.PostComment);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Environment.SetEnvironmentVariable(ActionOptionsFixture.AutoMergeEnvName, null);
        Environment.SetEnvironmentVariable(ActionOptionsFixture.CustomCommandEnvName, null);
        Environment.SetEnvironmentVariable(ActionOptionsFixture.PostCommentEnvName, null);
        
        return ValueTask.CompletedTask;
    }
    
    [Fact]
    public void SuccessfullySetsOptions()
    {
        IServiceProvider collection = new ServiceCollection()
            .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
            .BuildServiceProvider();

        IActionOptions actionOptions = collection.GetRequiredService<IActionOptions>();
        
        Assert.Equal(AssemblyFixture.AutoMerge, actionOptions.IsAutoMerge);
        Assert.Equal(AssemblyFixture.CustomCommand, actionOptions.CustomCommand);
        Assert.Equal(AssemblyFixture.PostComment, actionOptions.PostComment);
    }
}