using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Core.Exceptions;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitTests)]
public sealed class CloneRepo(GitTestFixture classFixture) : IClassFixture<GitTestFixture>
{
    private readonly string _tempDirectory = Path.Combine(
        classFixture.TempDirectory,
        nameof(Traits.Categories.Names.GitTests),
        nameof(CloneRepo));
    
    [Fact]
    public async Task SuccessfulRepoClone_DoesNotThrow()
    {
        IGit git = classFixture.BuildGit();
        string workingDir = Path.Combine(_tempDirectory, nameof(SuccessfulRepoClone_DoesNotThrow));
        string dirPath = Path.Combine(workingDir, ".git");
        Func<Task> action = () => git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        await action.Should().NotThrowAsync();
        
        bool dirExists = Directory.Exists(dirPath);
        dirExists.Should().BeTrue();
    }
    
    [Fact]
    public async Task FailedRepoClone_Throws()
    {
        IGit git = classFixture.BuildGit();
        string workingDir = Path.Combine(_tempDirectory, nameof(FailedRepoClone_Throws));
        Func<Task> action = () => git.CloneRepoAsync("i-am-an-invalid-repo-clone-url", workingDir);

        await action.Should().ThrowAsync<GitCommandException>();
    }

    [Fact]
    public async Task EmptyUrl_Throws()
    {
        IGit git = classFixture.BuildGit();
        string workingDir = Path.Combine(_tempDirectory, nameof(EmptyUrl_Throws));
        Func<Task> action = () => git.CloneRepoAsync(string.Empty, workingDir);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task EmptyWorkingDirectory_Throws()
    {
        IGit git = classFixture.BuildGit();
        Func<Task> action = () => git.CloneRepoAsync(classFixture.CloneUrl, string.Empty);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ForbiddenWorkingDirectory_Throws()
    {
        string workingDir = Path.GetTempPath();
        string full = Path.GetFullPath(workingDir).TrimEnd(Path.DirectorySeparatorChar);
        IGit git = classFixture.BuildGit();
        Func<Task> action = () => git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        (await action.Should().ThrowAsync<ArgumentException>()).WithMessage($"Refusing to use shared directory '{full}'. (Parameter 'workingDir')");
    }

    [Fact]
    public async Task NullRunnerTempEnvVar_DoesNotThrow()
    {
        string? original = Environment.GetEnvironmentVariable(classFixture.RunnerTempEnvName);
        Environment.SetEnvironmentVariable(classFixture.RunnerTempEnvName, null);
        IGit git = classFixture.BuildGit();
        string workingDir = Path.Combine(_tempDirectory, nameof(NullRunnerTempEnvVar_DoesNotThrow));
        Func<Task> action = () => git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        await action.Should().NotThrowAsync();
        
        Environment.SetEnvironmentVariable(classFixture.RunnerTempEnvName, original);
    }

    [Fact]
    public async Task RootWorkingDirectory_Throws()
    {
        IGit git = classFixture.BuildGit();
        string workingDir = "/";
        Func<Task> action = () => git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        await action.Should().ThrowAsync();
    }
}
