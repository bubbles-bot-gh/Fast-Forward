using JetBrains.Annotations;

namespace BlossomBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class CloneRepoFixture : IAsyncLifetime
{
    private static readonly string FixtureWorkingDir = $"{AssemblyFixture.GitTestsDir}/CloneRepo";
    public static string WorkingDir => $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public static string RepoUrl => AssemblyFixture.RepoUrl;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
    
    public static TheoryData<string, string> TestData =>
    [
        new TheoryDataRow<string, string>(RepoUrl, WorkingDir)
            .WithTestDisplayName("WithRepoAndWorkingDirectory"),
        
        new TheoryDataRow<string, string>(RepoUrl, "")
            .WithTestDisplayName("WithDefaultWorkingDirectory")
    ];
}