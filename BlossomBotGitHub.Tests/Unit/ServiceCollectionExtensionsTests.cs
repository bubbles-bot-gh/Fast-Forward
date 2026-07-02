using BlossomBotGitHub.FastForward.Core.ActionInfo;
using BlossomBotGitHub.FastForward.Core.GitHubApiCaller;
using BlossomBotGitHub.FastForward.Implements;
using Microsoft.Extensions.DependencyInjection;
using Fixture = BlossomBotGitHub.Tests.Fixtures.ServiceCollectionExtensionTests.ServiceCollectionExtensionsFixture;

namespace BlossomBotGitHub.Tests.Unit;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void Adds_IActionOptionsToServiceContainer_Successfully()
    {
        // Set env vars
        Environment.SetEnvironmentVariable(Fixture.IsAutoMergeEnvName, Fixture.IsAutoMerge.ToString().ToLower());
        Environment.SetEnvironmentVariable(Fixture.CustomCommandEnvName, Fixture.CustomCommand);
        Environment.SetEnvironmentVariable(Fixture.PostCommentEnvName, Fixture.PostComment);
        
        // Set up service container
        IServiceProvider services = new ServiceCollection()
            .AddAppServices()
            .BuildServiceProvider();
        
        IActionOptions actionOptions = services.GetRequiredService<IActionOptions>();

        Assert.Equal(Fixture.IsAutoMerge, actionOptions.IsAutoMerge);
        Assert.Equal(Fixture.CustomCommand, actionOptions.CustomCommand);
        Assert.Equal(Fixture.PostComment, actionOptions.PostComment);
    }

    [Fact]
    public void Adds_IGitHubApiCallerFactoryToServiceContainer_Successfully()
    {
        IServiceProvider services = new ServiceCollection()
            .AddAppServices()
            .BuildServiceProvider();
        
        IGitHubApiCallerFactory factory = services.GetRequiredService<IGitHubApiCallerFactory>();

        Assert.NotNull(factory);
    }
    
    
}