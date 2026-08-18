using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements;
using BubblesBotGitHub.Tests.Unit.Fixtures;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Tests;

[UsedImplicitly]
public sealed class ServiceCollectionExtensionsTests
{
    [Collection("GitHubApiCallerServiceTests")]
    public sealed class GitHubApiCallerService
    {
        [Fact]
        public void SucceedsWhenAddingFactoryService()
        {
            Mock<IGitHubApiCallerFactory> mockFactory = new();
            mockFactory
                .Setup(factory => factory.Create())
                .Returns(Mock.Of<IGitHubApiCaller>());
            
            IServiceProvider services = new ServiceCollection()
                .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
                .AddScoped<IGitHubApiCallerFactory>(_ => mockFactory.Object)
                .BuildServiceProvider();
        
            IGitHubApiCallerFactory factory = services.GetRequiredService<IGitHubApiCallerFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void SucceedsWhenAddingCallerService()
        {
            Mock<IGitHubApiCallerFactory> mockFactory = new();
            mockFactory
                .Setup(factory => factory.Create())
                .Returns(Mock.Of<IGitHubApiCaller>());
            
            IServiceProvider services = new ServiceCollection()
                .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
                .AddScoped<IGitHubApiCallerFactory>(_ => mockFactory.Object)
                .BuildServiceProvider();
            
            IGitHubApiCaller githubApiCaller = services.GetRequiredService<IGitHubApiCaller>();
            Assert.NotNull(githubApiCaller);
        }
    }
    
    [Collection("ActionInfoServiceTests")]
    public sealed class ActionInfoService
    {
        [Fact]
        public void SucceedsWhenAddingService()
        {
            Mock<IGitHubApiCallerFactory> mockFactory = new();
            mockFactory
                .Setup(factory => factory.Create())
                .Returns(Mock.Of<IGitHubApiCaller>());
            
            IServiceProvider services = new ServiceCollection()
                .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
                .AddScoped<IGitHubApiCallerFactory>(_ => mockFactory.Object)
                .BuildServiceProvider();
            
            IActionInfo actionInfo = services.GetRequiredService<IActionInfo>();
            
            Assert.NotNull(actionInfo);
        }
        
        [Fact]
        public void SucceedsWhenAddingIActionOptions()
        {
            // Set env vars
            Environment.SetEnvironmentVariable(ActionOptionsFixture.AutoMergeEnvName, AssemblyFixture.AutoMerge.ToString().ToLower());
            Environment.SetEnvironmentVariable(ActionOptionsFixture.CustomCommandEnvName, AssemblyFixture.CustomCommand);
            Environment.SetEnvironmentVariable(ActionOptionsFixture.PostCommentEnvName, AssemblyFixture.PostComment);
        
            // Set up service container
            IServiceProvider services = new ServiceCollection()
                .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
                .BuildServiceProvider();
        
            IActionOptions actionOptions = services.GetRequiredService<IActionOptions>();

            Assert.Equal(AssemblyFixture.AutoMerge, actionOptions.IsAutoMerge);
            Assert.Equal(AssemblyFixture.CustomCommand, actionOptions.CustomCommand);
            Assert.Equal(AssemblyFixture.PostComment, actionOptions.PostComment);

            Environment.SetEnvironmentVariable(ActionOptionsFixture.AutoMergeEnvName, null);
            Environment.SetEnvironmentVariable(ActionOptionsFixture.CustomCommandEnvName, null);
            Environment.SetEnvironmentVariable(ActionOptionsFixture.PostCommentEnvName, null);
        }
    }
}