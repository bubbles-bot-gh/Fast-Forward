namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

public class LogFixture : IAsyncLifetime
{
    private static readonly string FixtureWorkingDir = $"{AssemblyFixture.GitTestsDir}/Log";
    private static string Exclude => "8fbc7d5cd07170344e7d3404622f7f987163655a";
    public static string WorkingDir => $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
    
    public static TheoryData<string, string, string, string> TestData =>
    [
        new TheoryDataRow<string, string, string, string>(
                Exclude,
                AssemblyFixture.BaseSha,
                AssemblyFixture.HeadSha,
                WorkingDir)
            .WithTestDisplayName("WithExclude"),
        
        // With 'workingDirectory'
        new TheoryDataRow<string, string, string, string>(
            "",
            AssemblyFixture.BaseSha,
            AssemblyFixture.HeadSha,
            WorkingDir)
            .WithTestDisplayName("WithNoExclude")
    ];
}