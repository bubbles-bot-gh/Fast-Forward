using System.Reflection;
using BlossomBotGitHub.FastForward.Core;

namespace BlossomBotGitHub.Tests.Unit;

public class GitTest
{
    private readonly TestFixture _fixture;
    private readonly ITestOutputHelper _testOut;

    public GitTest(TestFixture fixture, ITestOutputHelper testOut)
    {
        _fixture = fixture;
        _testOut = testOut;
    }
    
    [Fact]
    public async Task RunsProcessAsync()
    {
        MethodInfo? subjectMethod = typeof(Git).GetMethod("RunProcessAsync", BindingFlags.Static | BindingFlags.NonPublic);
        if (subjectMethod == null) throw new NullReferenceException("RunProcessAsync");
        
        List<string> args = [ "hello world" ];
        if (subjectMethod
            .Invoke(null, ["echo", args, Environment.CurrentDirectory]) 
            is not Task<(int, string, string)> invokeResult) 
            throw new NullReferenceException();
        
        (int exitCode, string output, string error) result = await invokeResult;
        Assert.Equal("hello world", result.output.Trim());
    }

    public class CloneRepo
    {
        [Fact]
        public async Task ClonesRepoSuccessfully()
        {
            await Git.CloneRepoAsync("https://github.com/luneisolei/fast-forward-blossom-bot.git",
                "./tmp");
            Assert.True(Directory.Exists("./tmp/repo/.git"));
        }

        [Fact]
        public async Task ThrowsOnFail()
        {
            Task result = Git.CloneRepoAsync("nothing", "./tmp");
            await Assert.ThrowsAsync<Exception>(async () => await result);
        }
    }

    [Fact]
    public async Task Test()
    {
        _fixture.TestFunc(_testOut);
    }
}