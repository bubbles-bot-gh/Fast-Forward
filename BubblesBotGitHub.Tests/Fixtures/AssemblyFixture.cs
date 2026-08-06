using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using BubblesBotGitHub.Tests.Fixtures;
using Octokit;

[assembly: AssemblyFixture(typeof(AssemblyFixture))]
namespace BubblesBotGitHub.Tests.Fixtures;

public class AssemblyFixture
{
    internal IGitHubApiCaller CreateGitHubApiCaller(IGitHubClient client) => new GitHubApiCaller(client);
    
    private static readonly string RootWorkingDir = "./tmp";
    public static readonly string BaseSha = "a3f4edfee60026fc44989822ac8789e376f374a2";
    public static readonly string HeadSha = "1f85b89057373f54de739944889d1abec8c048b0";
    public static readonly string GitTestsDir = $"{RootWorkingDir}/GitTest";
    public static string RepoUrl => "https://github.com/LuneiSolei/Fast-Forward-Blossom-Bot-Tests.git";
}