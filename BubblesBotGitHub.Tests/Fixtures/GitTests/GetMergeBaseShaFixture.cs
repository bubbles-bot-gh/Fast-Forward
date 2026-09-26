using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class GetMergeBaseShaFixture : IAsyncLifetime
{
    public readonly string WorkingDir = $"{AssemblyFixture.RootWorkingDir}/GitTests/GetMergeBaseSha/";
    public readonly IGit Subject = new Git(new ProcessOutFactory());
    public readonly string TestRepoBaseSha = AssemblyFixture.TestRepoBaseSha;
    public readonly string TestRepoHeadSha = AssemblyFixture.TestRepoHeadSha;
    public string ExpectedMergeTestRepoBaseSha => AssemblyFixture.TestRepoBaseSha;
    public string InvalidSha => "1";
    public readonly string TestRepoUrl = AssemblyFixture.TestRepoUrl;

    public async ValueTask InitializeAsync()
    {
        await Subject.CloneRepoAsync(TestRepoUrl, WorkingDir);
    }

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: WorkingDir, recursive: true);
        
        return ValueTask.CompletedTask;
    }
}