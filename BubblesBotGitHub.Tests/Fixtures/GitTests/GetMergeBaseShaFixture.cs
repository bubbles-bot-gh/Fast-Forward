using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Implements.Git;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitTests;

[UsedImplicitly]
public class GetMergeBaseShaFixture
{
    private static readonly string FixtureWorkingDir = $"{AssemblyFixture.GitTestsDir}/GetMergeBaseSha/";
    public static string WorkingDir => $"{FixtureWorkingDir}/{Guid.NewGuid()}";
    public static string BaseSha => AssemblyFixture.BaseSha;
    public static string HeadSha => AssemblyFixture.HeadSha;
    public static string ExpectedMergeBaseSha => AssemblyFixture.BaseSha;
    public static string InvalidSha => "1";
    public static string RepoUrl => AssemblyFixture.RepoUrl;
    public readonly IGit Subject = new Git(new ProcessOutFactory());
}