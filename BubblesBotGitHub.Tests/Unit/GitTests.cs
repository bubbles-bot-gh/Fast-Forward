using BubblesBotGitHub.FastForward.Core.Errors;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Implements.Git;
using BubblesBotGitHub.Tests.Fixtures;
using BubblesBotGitHub.Tests.Fixtures.GitTests;
using JetBrains.Annotations;
namespace BubblesBotGitHub.Tests.Unit;

[UsedImplicitly]
public sealed class GitTests
{
    public sealed class CloneRepo(CloneRepoFixture classFixture)
        : IAsyncLifetime, IClassFixture<CloneRepoFixture>
    {
        private string WorkingDir { get; } = CloneRepoFixture.WorkingDir;
        
        public ValueTask InitializeAsync() => ValueTask.CompletedTask;
        
        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(WorkingDir))
                Directory.Delete(path: WorkingDir, recursive: true);
            
            return ValueTask.CompletedTask;
        }
        
        [Fact]
        public async Task ClonesRepo_IntoWorkingDirectory_Successfully()
        {
            await classFixture.Subject.CloneRepoAsync(CloneRepoFixture.RepoUrl, WorkingDir);
            
            Assert.True(Directory.Exists($"{WorkingDir}/.git"));
        }
    
        [Fact]
        public async Task FailsOnInvalidRepo()
        {
            Task result = classFixture.Subject.CloneRepoAsync(
                cloneUrl: "IAmAnInvalidRepo",
                workingDir: WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }

        [Fact]
        public async Task Throws_OnWhiteSpaceUrl()
        {
            Task result = classFixture.Subject.CloneRepoAsync(
                cloneUrl: "",
                workingDir: WorkingDir);
            
            await Assert.ThrowsAsync<ArgumentException>(async () => await result);
        }

        [Fact]
        public async Task Throws_OnWhiteSpaceWorkingDirectory()
        {
            Task result = classFixture.Subject.CloneRepoAsync(
                cloneUrl: AssemblyFixture.RepoUrl,
                workingDir: "");
            
            await Assert.ThrowsAsync<ArgumentException>(async () => await result);
        }

        [Fact]
        public async Task RemovesDuplicateDirectory()
        {
            // Create test file
            string testFilePath = $"{WorkingDir}/RemovesDuplicateDirectoryTest.txt";
            string nestedDirPath = $"{WorkingDir}/NestedDir";
            Directory.CreateDirectory(nestedDirPath);
            File.Create(testFilePath);

            // Clone the repo
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: CloneRepoFixture.RepoUrl,
                workingDir: WorkingDir);
            
            Assert.False(Directory.Exists(testFilePath));
        }
    }

    public sealed class Log(ITestOutputHelper testOutput, LogFixture classFixture)
    : IAsyncLifetime, IClassFixture<LogFixture>
    {
        private string WorkingDir { get; } = LogFixture.WorkingDir;
        public async ValueTask InitializeAsync()
        {
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: AssemblyFixture.RepoUrl,
                workingDir: WorkingDir);
        }
        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(WorkingDir))
                Directory.Delete(path: WorkingDir, recursive: true);
            
            return ValueTask.CompletedTask;
        }
        
        [Theory]
        [MemberData(nameof(LogFixture.TestData), MemberType = typeof(LogFixture))]
        public async Task Succeeds(
            string exclude,
            string baseSha,
            string headSha,
            string workingDir)
        {
            testOutput.WriteLine(WorkingDir);
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: AssemblyFixture.RepoUrl,
                workingDir: workingDir);
            
            string result = await classFixture.Subject.Log(
                exclude: exclude,
                baseSha: baseSha,
                headSha: headSha,
                workingDir: workingDir);
            testOutput.WriteLine(result);
            Directory.Delete(path: workingDir, recursive: true);
            
            Assert.NotEmpty(result);
        }
    
        [Fact]
        public async Task FailsWithInvalidSha()
        {
            Task<string> result = classFixture.Subject.Log(
                exclude: "",
                baseSha: "IAmAnInvalidSHA",
                headSha: "IAmAnInvalidSHAToo\"",
                workingDir: "");
    
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }
    }

    public sealed class GetMergeBaseSha(GetMergeBaseShaFixture classFixture)
        : IClassFixture<GetMergeBaseShaFixture>, IAsyncLifetime
    {
        private string WorkingDir { get; } = GetMergeBaseShaFixture.WorkingDir;

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }
    
        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(WorkingDir))
                Directory.Delete(path: WorkingDir, recursive: true);
            
            return ValueTask.CompletedTask;
        }
    
        [Fact]
        public async Task GetsShaSuccessfully()
        {
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: GetMergeBaseShaFixture.RepoUrl,
                workingDir: WorkingDir);
            
            string result = await classFixture.Subject.GetMergeBaseSha(
                baseSha: GetMergeBaseShaFixture.BaseSha, 
                headSha: GetMergeBaseShaFixture.HeadSha,
                workingDir: WorkingDir);
            
            Assert.Equal(GetMergeBaseShaFixture.ExpectedMergeBaseSha, result);
        }
    
        [Fact]
        public async Task FailsToGetSha()
        {
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: GetMergeBaseShaFixture.RepoUrl,
                WorkingDir);
            
            Task<string> result = classFixture.Subject.GetMergeBaseSha(
                baseSha: GetMergeBaseShaFixture.BaseSha, 
                headSha: GetMergeBaseShaFixture.InvalidSha,
                WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }
    }
    
    public sealed class GetAmountOfParents(GetAmountOfParentsFixture classFixture)
        : IAsyncLifetime, IClassFixture<GetAmountOfParentsFixture>
    {
        private string WorkingDir { get; } = GetAmountOfParentsFixture.WorkingDir;
    
        public async ValueTask InitializeAsync()
        {
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: AssemblyFixture.RepoUrl,
                workingDir: WorkingDir);
        }
    
        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(WorkingDir))
                Directory.Delete(WorkingDir, recursive: true);
            
            return ValueTask.CompletedTask;
        }
    
        [Fact]
        public async Task GetsAmountOfParentsSuccessfully()
        {
            uint parents = await classFixture.Subject.GetAmountOfParents(
                GetAmountOfParentsFixture.Sha, WorkingDir);
            
            Assert.Equal(GetAmountOfParentsFixture.ExpectedAmount, parents);
        }
    
        [Fact]
        public async Task FailsOnNonZeroExitCode()
        {
            Task<uint> result = classFixture.Subject.GetAmountOfParents(GetAmountOfParentsFixture.InvalidSha, WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }

        [Fact]
        public async Task ReturnsZero_OnEmptyString()
        {
            uint result = await classFixture.Subject.GetAmountOfParents(AssemblyFixture.BaseSha, WorkingDir);
            const uint expected = 0;
            
            Assert.Equal(expected, result);
        }
    }
}
