using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements;
using BubblesBotGitHub.Tests.Fixtures;
using BubblesBotGitHub.Tests.Fixtures.ServiceCollectionExtensionTests;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BubblesBotGitHub.Tests.Unit;

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
            Environment.SetEnvironmentVariable(ActionOptionsFixture.IsAutoMergeEnvName, ActionOptionsFixture.IsAutoMerge.ToString().ToLower());
            Environment.SetEnvironmentVariable(ActionOptionsFixture.CustomCommandEnvName, ActionOptionsFixture.CustomCommand);
            Environment.SetEnvironmentVariable(ActionOptionsFixture.PostCommentEnvName, ActionOptionsFixture.PostComment);
        
            // Set up service container
            IServiceProvider services = new ServiceCollection()
                .AddAppServices(AssemblyFixture.CreatePullRequestOpenedEvent())
                .BuildServiceProvider();
        
            IActionOptions actionOptions = services.GetRequiredService<IActionOptions>();

            Assert.Equal(ActionOptionsFixture.IsAutoMerge, actionOptions.IsAutoMerge);
            Assert.Equal(ActionOptionsFixture.CustomCommand, actionOptions.CustomCommand);
            Assert.Equal(ActionOptionsFixture.PostComment, actionOptions.PostComment);

            Environment.SetEnvironmentVariable(ActionOptionsFixture.IsAutoMergeEnvName, null);
            Environment.SetEnvironmentVariable(ActionOptionsFixture.CustomCommandEnvName, null);
            Environment.SetEnvironmentVariable(ActionOptionsFixture.PostCommentEnvName, null);
        }
    }
}