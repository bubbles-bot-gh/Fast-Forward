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
public sealed class LogCommitGraph(GitTestFixture classFixture) : IClassFixture<GitTestFixture>
{
    private readonly string _tempDirectory = Path.Combine(
        classFixture.TempDirectory,
        nameof(Traits.Categories.Names.GitTests),
        nameof(LogCommitGraph));
    
    [Theory]
    [MemberData(nameof(GitTestFixture.TheoryData), MemberType = typeof(GitTestFixture))]
    public async Task ValidInputData_DoesNotThrow(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        HostApplicationBuilder builder = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices { MockGit = null, MockProcessOutFactory = null });
        IHost host = builder.Build();
        
        string workingDir = Path.Combine(_tempDirectory, nameof(ValidInputData_DoesNotThrow));
        IGit git = host.Services.GetRequiredService<IGit>();
        await git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        string? result = null;
        Func<Task<string>> action = async () =>
        {
            result = await git.LogCommitGraph(classFixture.ExcludeCommit, classFixture.BaseSha,
                classFixture.HeadSha, workingDir);

            return result;
        };

        await action.Should().NotThrowAsync();
        result.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(GitTestFixture.TheoryData), MemberType = typeof(GitTestFixture))]
    public async Task InvalidSha_Throws(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IHost host = classFixture.CreateApplicationBuilderWithMocks(webhookEvent, eventType, new MockServices
            {
                MockGit = null,
                MockProcessOutFactory = null
            }).Build();

        string workingDir = Path.Combine(_tempDirectory, nameof(InvalidSha_Throws));
        IGit git = host.Services.GetRequiredService<IGit>();
        await git.CloneRepoAsync(classFixture.CloneUrl, workingDir);
        
        Func<Task<string>> action = () => git.LogCommitGraph(classFixture.ExcludeCommit, classFixture.InvalidSha, 
                classFixture.InvalidSha, workingDir);

        await action.Should().ThrowAsync<GitCommandException>();
    }
}
