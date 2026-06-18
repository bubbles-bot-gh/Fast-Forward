using System.ComponentModel;
using System.Reflection;
using BlossomBotGitHub.FastForward.Core;
using BlossomBotGitHub.FastForward.Core.Git;

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
    
    public class RunProcessAsync
    {
        private readonly MethodInfo _subjectMethod = typeof(Git).GetMethod(
            name: "RunProcessAsync", 
            bindingAttr: BindingFlags.Static | BindingFlags.NonPublic) 
            ?? throw new NullReferenceException("RunProcessAsync");

        [Fact]
        public async Task RunsProcessSuccessfully()
        {
            List<string> args = [ "hello world" ];
            object invokeResult = _subjectMethod.Invoke(null, ["echo", args, Environment.CurrentDirectory]) 
                                  ?? throw new NullReferenceException();

            Task<IProcessOut> taskResult = invokeResult as Task<IProcessOut>
                                                     ?? throw new NullReferenceException();
        
            IProcessOut result = await taskResult;
            Assert.Equal("hello world", result.StdOut.Trim());
        }

        [Fact]
        public async Task FailsToStartProcess()
        {
            List<string> args = [ "hello world" ];
            string fakeExe = $"nonexistant-{Guid.NewGuid():N}";
            object invokeResult = _subjectMethod.Invoke(null, [ fakeExe, args, Environment.CurrentDirectory ]) 
                            ?? throw new InvalidOperationException();
            
            Task taskResult = invokeResult as Task ?? throw new InvalidOperationException();

            await Assert.ThrowsAsync<Win32Exception>(async () => await taskResult);
        }
    }

    public class DeleteRecursive
    {
        private readonly MethodInfo _subjectMethod = typeof(Git).GetMethod(
            name: "DeleteRecursive",
            bindingAttr: BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new NullReferenceException("DeleteRecursive");

        [Fact]
        public void DeletesDirectorySuccessfully()
        {
            // Create test directory
            Directory.CreateDirectory("./tmp/DeleteRecursive_SuccessTest");
            _subjectMethod.Invoke(null, ["./tmp/DeleteRecursive_SuccessTest"]);
            
            // Verify directory is deleted
            bool exists = Directory.Exists("./tmp/DeleteRecursive_SuccessTest");
            Assert.False(exists);
        }

        [Fact]
        public void FailsToDeleteDirectory()
        {
            Directory.CreateDirectory("./tmp/DeleteRecursive_FailTest");
            _subjectMethod.Invoke(null, ["./tmp/DeleteRecursive_FailTest"]);
            
            // Verify directory is not deleted
            bool exists = Directory.Exists("./tmp/DeleteRecursive_FailTest");
            Assert.True(exists);
            
            // Actually remove directory
            Directory.Delete("./tmp/DeleteRecursive_FailTest");
        }
    }
    
    public class CloneRepo
    {
        [Fact]
        public async Task ClonesRepoSuccessfully()
        {
            await CloneRepoAsync("https://github.com/luneisolei/fast-forward-blossom-bot.git",
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
}