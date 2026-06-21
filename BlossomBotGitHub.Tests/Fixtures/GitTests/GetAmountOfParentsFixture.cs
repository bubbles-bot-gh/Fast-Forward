using JetBrains.Annotations;

namespace BlossomBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class GetAmountOfParentsFixture : IAsyncLifetime
{
    private static readonly string FixtureWorkingDir = $"{AssemblyFixture.GitTestsDir}/GetAmountOfParents";
    public static string Sha => AssemblyFixture.HeadSha;
    public static uint ExpectedAmount => 1;
    public static string WorkingDir => $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public static string InvalidSha => "1";
    
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
}