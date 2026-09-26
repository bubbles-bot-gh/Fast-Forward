using System.Diagnostics.CodeAnalysis;
using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Services;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

public class LogCommitGraphTestsFixture : IAsyncLifetime
{
    private const string FixtureWorkingDir = $"{AssemblyFixture.RootWorkingDir}/GitTests/Log";
    private const string ExcludeCommit = "8fbc7d5cd07170344e7d3404622f7f987163655a";
    private const string TestRepoBaseSha = AssemblyFixture.TestRepoBaseSha;
    private const string TestRepoHeadSha = AssemblyFixture.TestRepoHeadSha;
    
    public readonly string WorkingDir = $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public readonly IGit Subject = new Git(new ProcessOutFactory());
    public readonly string TestRepoUrl = AssemblyFixture.TestRepoUrl;

    [ExcludeFromCodeCoverage]
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    [ExcludeFromCodeCoverage]
    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: FixtureWorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
    
    public static TheoryData<string, string, string, string> TheoryData =>
    [
        new TheoryDataRow<string, string, string, string>(
                ExcludeCommit,
                TestRepoBaseSha,
                TestRepoHeadSha,
                $"{FixtureWorkingDir}/{Guid.NewGuid()}")
            .WithTestDisplayName("WithExclude"),
        
        // With 'workingDirectory'
        new TheoryDataRow<string, string, string, string>(
            string.Empty,
            TestRepoBaseSha,
            TestRepoHeadSha,
            $"{FixtureWorkingDir}/{Guid.NewGuid()}")
            .WithTestDisplayName("WithNoExclude")
    ];
}