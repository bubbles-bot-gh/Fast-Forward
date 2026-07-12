using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Implements.Git;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class GetAmountOfParentsFixture : IAsyncLifetime
{
    private static readonly string FixtureWorkingDir = $"{AssemblyFixture.GitTestsDir}/GetAmountOfParents";
    public static string Sha => AssemblyFixture.HeadSha;
    public static uint ExpectedAmount => 1;
    public static string WorkingDir => $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public static string InvalidSha => "1";
    public readonly IGit Subject = new Git(new ProcessOutFactory());
    
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
}