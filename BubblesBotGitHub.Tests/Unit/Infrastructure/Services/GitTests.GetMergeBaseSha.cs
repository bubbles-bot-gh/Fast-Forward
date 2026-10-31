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
public sealed class GetMergeBaseSha(GitTestFixture classFixture) : IClassFixture<GitTestFixture>
{
    private readonly string _tempDirectory = Path.Combine(
        classFixture.TempDirectory,
        nameof(Traits.Categories.Names.GitTests),
        nameof(GetMergeBaseSha));
    
    [Theory]
    [MemberData(nameof(GitTestFixture.TheoryData), MemberType = typeof(GitTestFixture))]
    public async Task ValidSha_DoesNotThrow(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IHost host = classFixture.CreateApplicationBuilderWithMocks(webhookEvent, eventType, new MockServices
        {
            MockGit = null,
            MockProcessOutFactory = null
        }).Build();

        string workingDir = Path.Combine(_tempDirectory, nameof(ValidSha_DoesNotThrow));
        IGit git = host.Services.GetRequiredService<IGit>();
        await git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        Func<Task<string>> action = () => git.GetMergeBaseSha(
            classFixture.BaseSha,
            classFixture.HeadSha,
            workingDir);

        await action.Should().NotThrowAsync();
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

        Func<Task<string>> action = () => git.GetMergeBaseSha(
            classFixture.InvalidSha,
            classFixture.InvalidSha,
            workingDir);

        await action.Should().ThrowAsync<GitCommandException>();
    }
}