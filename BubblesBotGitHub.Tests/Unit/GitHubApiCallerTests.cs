using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace BubblesBotGitHub.Tests.Unit;

public sealed class GitHubApiCallerTests(AssemblyFixture assemblyFixture)
{
    [Fact]
    public void CreateApiCaller()
    {
        // IGitHubApiCallerFactory apiCaller = assemblyFixture.Services.GetService<IGitHubApiCallerFactory>()
        //     ?? throw new InvalidOperationException("Cannot get service of type 'IGitApiCallerFactory'.");
        //
        // Assert.NotNull(apiCaller);
    }
}