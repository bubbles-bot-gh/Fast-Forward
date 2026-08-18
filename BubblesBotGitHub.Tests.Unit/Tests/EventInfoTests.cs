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
        // Create mock GitHubApiCaller
        Mock<IGitHubApiCallerFactory> mockFactory = new();
        mockFactory
            .Setup(factory => factory.Create())
            .Returns(Mock.Of<IGitHubApiCaller>());
        
        // Create mock RepoInfo
        Mock<IRepoInfo> mockRepoInfo = new();
        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.Owner)
            .Returns(EventInfoFixture.Owner);
        
        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.Name)
            .Returns(EventInfoFixture.Name);

        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.CloneUrl)
            .Returns(EventInfoFixture.CloneUrl);
        
        IServiceProvider serviceProvider = new ServiceCollection()
            .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
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
        Assert.Equal(_eventInfo.CommentBody, AssemblyFixture.CreatePullRequestOpenedEvent().PullRequest.Body);
    }
}