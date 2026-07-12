using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Implements.Git;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class CloneRepoFixture : IAsyncLifetime
{
    public readonly IGit Subject = new Git(new ProcessOutFactory());
    private static readonly string FixtureWorkingDir = $"{AssemblyFixture.GitTestsDir}/CloneRepo";
    public static string WorkingDir => $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public static string RepoUrl => AssemblyFixture.RepoUrl;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
}