using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class GetAmountOfParentsFixture : IAsyncLifetime
{
    public readonly string WorkingDir = $"{AssemblyFixture.RootWorkingDir}/GitTests/GetAmountOfParents";
    public readonly string TestRepoHeadSha = AssemblyFixture.TestRepoHeadSha;
    public readonly string TestRepoBaseSha = AssemblyFixture.TestRepoBaseSha;
    public readonly uint ExpectedAmount = 1;
    public readonly string InvalidSha = "1";
    public readonly IGit Subject = new Git(new ProcessOutFactory());
    public readonly string TestRepoUrl = AssemblyFixture.TestRepoUrl;

    public async ValueTask InitializeAsync()
    {
        await Subject.CloneRepoAsync(TestRepoUrl, WorkingDir);
    }

    public ValueTask DisposeAsync()
    {
        Directory.Delete(path: WorkingDir, recursive: true);
        GC.SuppressFinalize(this);
        
        return ValueTask.CompletedTask;
    }
}