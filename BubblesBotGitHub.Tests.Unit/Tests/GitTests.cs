using BubblesBotGitHub.FastForward.Core.Errors;
using BubblesBotGitHub.Tests.Unit.Fixtures.GitTests;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Unit.Tests;

[UsedImplicitly]
public sealed class GitTests
{
    [Collection("CloneRepoTests")]
    public sealed class CloneRepo(CloneRepoFixture classFixture)
        : IAsyncLifetime, IClassFixture<CloneRepoFixture>
    {
        public ValueTask InitializeAsync() => ValueTask.CompletedTask;
        
        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(classFixture.WorkingDir))
                Directory.Delete(path: classFixture.WorkingDir, recursive: true);
            
            return ValueTask.CompletedTask;
        }
        
        [Fact]
        public async Task SucceedsWhenCloningRepo()
        {
            await classFixture
                .Subject
                .CloneRepoAsync(classFixture.TestRepoUrl, classFixture.WorkingDir);
            
            Assert.True(Directory.Exists($"{classFixture.WorkingDir}/.git"));
        }
    
        [Fact]
        public async Task ThrowsWhenCloningRepoFails()
        {
            Task result = classFixture
                .Subject
                .CloneRepoAsync("IAmAnInvalidRepo", classFixture.WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }

        [Fact]
        public async Task ThrowsWhenUrlIsEmpty()
        {
            Task result = classFixture.Subject.CloneRepoAsync(
                cloneUrl: string.Empty,
                workingDir: classFixture.WorkingDir);
            
            await Assert.ThrowsAsync<ArgumentException>(async () => await result);
        }

        [Fact]
        public async Task ThrowsWhenWorkingDirectoryPathIsEmpty()
        {
            Task result = classFixture.Subject.CloneRepoAsync(
                cloneUrl: classFixture.TestRepoUrl,
                workingDir: string.Empty);
            
            await Assert.ThrowsAsync<ArgumentException>(async () => await result);
        }

        [Fact]
        public async Task SucceedsWhenRemovingDuplicateDirectory()
        {
            // Create test file
            string testFilePath = $"{classFixture.WorkingDir}/RemovesDuplicateDirectoryTest.txt";
            string nestedDirPath = $"{classFixture.WorkingDir}/NestedDir";
            Directory.CreateDirectory(nestedDirPath);
            File.Create(testFilePath);

            // Clone the repo
            await classFixture.Subject.CloneRepoAsync(
                cloneUrl: classFixture.TestRepoUrl,
                workingDir: classFixture.WorkingDir);
            
            Assert.False(Directory.Exists(testFilePath));
        }
    }

    [Collection("LogCommitGraphTests")]
    public sealed class LogCommitGraph(ITestOutputHelper testOutput, LogCommitGraphTestsFixture classCommitGraphTestsFixture)
    : IAsyncLifetime, IClassFixture<LogCommitGraphTestsFixture>
    {
        public async ValueTask InitializeAsync()
        {
            await classCommitGraphTestsFixture.Subject.CloneRepoAsync(
                cloneUrl: classCommitGraphTestsFixture.TestRepoUrl,
                workingDir: classCommitGraphTestsFixture.WorkingDir);
        }
        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(classCommitGraphTestsFixture.WorkingDir))
                Directory.Delete(path: classCommitGraphTestsFixture.WorkingDir, recursive: true);
            
            return ValueTask.CompletedTask;
        }
        
        [Theory]
        [MemberData(nameof(LogCommitGraphTestsFixture.TheoryData), MemberType = typeof(LogCommitGraphTestsFixture))]
        public async Task Succeeds(
            string exclude,
            string baseSha,
            string headSha,
            string workingDir)
        {
            testOutput.WriteLine(classCommitGraphTestsFixture.WorkingDir);
            await classCommitGraphTestsFixture.Subject.CloneRepoAsync(
                cloneUrl: classCommitGraphTestsFixture.TestRepoUrl,
                workingDir: workingDir);
            
            string result = await classCommitGraphTestsFixture.Subject.LogCommitGraph(
                exclude,
                baseSha,
                headSha,
                workingDir);

            Directory.Delete(path: workingDir, recursive: true);
            
            Assert.NotEmpty(result);
        }
    
        [Fact]
        public async Task FailsWithInvalidSha()
        {
            Task<string> result = classCommitGraphTestsFixture.Subject.LogCommitGraph(
                exclude: "",
                baseSha: "IAmAnInvalidSHA",
                headSha: "IAmAnInvalidSHAToo\"",
                workingDir: "");
    
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }
    }

    [Collection("GetMergeBaseShaTests")]
    public sealed class GetMergeBaseSha(GetMergeBaseShaFixture classFixture)
        : IClassFixture<GetMergeBaseShaFixture>
    {
        [Fact]
        public async Task GetsShaSuccessfully()
        {
            string result = await classFixture.Subject.GetMergeBaseSha(
                classFixture.TestRepoBaseSha, 
                classFixture.TestRepoHeadSha,
                classFixture.WorkingDir);
            
            Assert.Equal(classFixture.ExpectedMergeTestRepoBaseSha, result);
        }
    
        [Fact]
        public async Task FailsToGetSha()
        {
            Task<string> result = classFixture.Subject.GetMergeBaseSha(
                classFixture.TestRepoBaseSha, 
                classFixture.InvalidSha,
                classFixture.WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }
    }
    
    [Collection("GetAmountOfParentsTests")]
    public sealed class GetAmountOfParents(GetAmountOfParentsFixture classFixture)
        : IClassFixture<GetAmountOfParentsFixture>
    {
        [Fact]
        public async Task GetsAmountOfParentsSuccessfully()
        {
            uint parents = await classFixture.Subject.GetAmountOfParents(
                classFixture.TestRepoHeadSha, classFixture.WorkingDir);
            
            Assert.Equal(classFixture.ExpectedAmount, parents);
        }
    
        [Fact]
        public async Task FailsOnNonZeroExitCode()
        {
            Task<uint> result = classFixture.Subject.GetAmountOfParents(classFixture.InvalidSha, classFixture.WorkingDir);
            
            await Assert.ThrowsAsync<GitCommandException>(async () => await result);
        }

        [Fact]
        public async Task ReturnsZero_OnEmptyString()
        {
            uint result = await classFixture.Subject.GetAmountOfParents(classFixture.TestRepoBaseSha, classFixture.WorkingDir);
            const uint expected = 0;
            
            Assert.Equal(expected, result);
        }
    }
}
