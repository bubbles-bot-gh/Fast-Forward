using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class CloneRepoFixture : IAsyncLifetime
{
    private const string FixtureWorkingDir = $"{AssemblyFixture.RootWorkingDir}/GitTests/CloneRepo";
    public readonly string WorkingDir = $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public readonly IGit Subject = new Git(new ProcessOutFactory());
    
    [UsedImplicitly]
    public readonly string TestRepoUrl = AssemblyFixture.TestRepoUrl;
    
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        GC.SuppressFinalize(this);
        
        return ValueTask.CompletedTask;
    }
}