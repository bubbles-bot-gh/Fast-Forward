using BubblesBotGitHub.FastForward.Core.Errors;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.Tests.Fixtures;
using BubblesBotGitHub.Tests.Fixtures.GitTests;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
namespace BubblesBotGitHub.Tests.Unit;

[UsedImplicitly]
public sealed class GitTests
{
    public sealed class CloneRepo(AssemblyFixture assemblyFixture) 
        : IAsyncLifetime, IClassFixture<CloneRepoFixture>
    {
        private readonly IGit _subject = assemblyFixture.Services.GetService<IGit>() 
            ?? throw new InvalidOperationException("Cannot get service of type 'IGit'.");

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
            await _subject.CloneRepoAsync(CloneRepoFixture.RepoUrl, WorkingDir);
            
            Assert.True(Directory.Exists($"{WorkingDir}/.git"));
        }
    
        [Fact]
        public async Task FailsOnInvalidRepo()
        {
            Task result = _subject.CloneRepoAsync(
                cloneUrl: "IAmAnInvalidRepo",
                workingDir: WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }

        [Fact]
        public async Task Throws_OnWhiteSpaceUrl()
        {
            Task result = _subject.CloneRepoAsync(
                cloneUrl: "",
                workingDir: WorkingDir);
            
            await Assert.ThrowsAsync<ArgumentException>(async () => await result);
        }

        [Fact]
        public async Task Throws_OnWhiteSpaceWorkingDirectory()
        {
            Task result = _subject.CloneRepoAsync(
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
            await _subject.CloneRepoAsync(
                cloneUrl: CloneRepoFixture.RepoUrl,
                workingDir: WorkingDir);
            
            Assert.False(Directory.Exists(testFilePath));
        }
    }

    public sealed class Log : IAsyncLifetime, IClassFixture<LogFixture>
    {
        private readonly IGit _subject;
        private readonly ITestOutputHelper _testOutput;
        private string WorkingDir { get; } = LogFixture.WorkingDir;
    
        public Log(AssemblyFixture assemblyFixture, ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _subject = assemblyFixture.Services.GetService<IGit>() 
                ?? throw new InvalidOperationException("Cannot get service of type 'IGit'.");
        }

        public async ValueTask InitializeAsync()
        {
            await _subject.CloneRepoAsync(
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
            _testOutput.WriteLine(WorkingDir);
            await _subject.CloneRepoAsync(
                cloneUrl: AssemblyFixture.RepoUrl,
                workingDir: workingDir);
            
            string result = await _subject.Log(
                exclude: exclude,
                baseSha: baseSha,
                headSha: headSha,
                workingDir: workingDir);
            _testOutput.WriteLine(result);
            Directory.Delete(path: workingDir, recursive: true);
            
            Assert.NotEmpty(result);
        }
    
        [Fact]
        public async Task FailsWithInvalidSha()
        {
            Task<string> result = _subject.Log(
                exclude: "",
                baseSha: "IAmAnInvalidSHA",
                headSha: "IAmAnInvalidSHAToo\"",
                workingDir: "");
    
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }
    }

    public sealed class GetMergeBaseSha(AssemblyFixture assemblyFixture)
        : IClassFixture<GetMergeBaseShaFixture>, IAsyncLifetime
    {
        private readonly IGit _subject = assemblyFixture.Services.GetService<IGit>() 
            ?? throw new InvalidOperationException("Cannot get service of type 'IGit'.");
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
            await _subject.CloneRepoAsync(
                cloneUrl: GetMergeBaseShaFixture.RepoUrl,
                workingDir: WorkingDir);
            
            string result = await _subject.GetMergeBaseSha(
                baseSha: GetMergeBaseShaFixture.BaseSha, 
                headSha: GetMergeBaseShaFixture.HeadSha,
                workingDir: WorkingDir);
            
            Assert.Equal(GetMergeBaseShaFixture.ExpectedMergeBaseSha, result);
        }
    
        [Fact]
        public async Task FailsToGetSha()
        {
            await _subject.CloneRepoAsync(
                cloneUrl: GetMergeBaseShaFixture.RepoUrl,
                WorkingDir);
            
            Task<string> result = _subject.GetMergeBaseSha(
                baseSha: GetMergeBaseShaFixture.BaseSha, 
                headSha: GetMergeBaseShaFixture.InvalidSha,
                WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }
    }
    
    public sealed class GetAmountOfParents(AssemblyFixture assemblyFixture)
        : IAsyncLifetime, IClassFixture<GetAmountOfParentsFixture>
    {
        private readonly IGit _subject = assemblyFixture.Services.GetService<IGit>() 
            ?? throw new InvalidOperationException("Cannot get service of type 'IGit'.");
        private string WorkingDir { get; } = GetAmountOfParentsFixture.WorkingDir;
    
        public async ValueTask InitializeAsync()
        {
            await _subject.CloneRepoAsync(
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
            uint parents = await _subject.GetAmountOfParents(
                GetAmountOfParentsFixture.Sha, WorkingDir);
            
            Assert.Equal(GetAmountOfParentsFixture.ExpectedAmount, parents);
        }
    
        [Fact]
        public async Task FailsOnNonZeroExitCode()
        {
            Task<uint> result = _subject.GetAmountOfParents(GetAmountOfParentsFixture.InvalidSha, WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }

        [Fact]
        public async Task ReturnsZero_OnEmptyString()
        {
            uint result = await _subject.GetAmountOfParents(AssemblyFixture.BaseSha, WorkingDir);
            const uint expected = 0;
            
            Assert.Equal(expected, result);
        }
    }
}
