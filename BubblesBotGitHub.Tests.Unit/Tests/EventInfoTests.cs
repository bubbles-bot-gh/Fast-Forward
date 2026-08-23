using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements;
using BubblesBotGitHub.Tests.Unit.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Tests;

public sealed class EventInfoTests : IAsyncLifetime
{
    private IEventInfo _eventInfo = null!;
    
    public ValueTask InitializeAsync()
    {
        // Create mocks
        Mock<IGitHubApiCallerFactory> mockFactory = AssemblyFixture.CreateMockGitHubApiCallerFactory();
        Mock<IRepoInfo> mockRepoInfo = AssemblyFixture.CreateMockRepoInfo();
        
        IServiceProvider serviceProvider = new ServiceCollection()
            .AddAppServices(AssemblyFixture.GetPullRequestOpenedEvent(), ActionEventType.PullRequestOpened)
            .AddScoped<IGitHubApiCallerFactory>(_ => mockFactory.Object)
            .AddScoped<IRepoInfo>(_ => mockRepoInfo.Object)
            .BuildServiceProvider();
        
        _eventInfo = serviceProvider.GetRequiredService<IEventInfo>();

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
    
    [Fact]
    public void CommentBodyExtractionSucceeds()
    {
        Assert.Equal(_eventInfo.CommentBody, AssemblyFixture.GetPullRequestOpenedEvent().PullRequest.Body);
    }
}