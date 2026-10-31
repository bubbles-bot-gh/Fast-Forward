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
public sealed class GetParentCount(GitTestFixture classFixture) : IClassFixture<GitTestFixture>
{
    private readonly string _tempDirectory = Path.Combine(
        classFixture.TempDirectory,
        nameof(Traits.Categories.Names.GitTests),
        nameof(GetParentCount));
    
    [Theory]
    [MemberData(nameof(GitTestFixture.TheoryData), MemberType = typeof(GitTestFixture))]
    public async Task ValidArgs_DoesNotThrow(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices
            {
                MockGit = null,
                MockProcessOutFactory = null
            }).Build();

        string workingDir = Path.Combine(_tempDirectory, nameof(ValidArgs_DoesNotThrow));
        IGit git = host.Services.GetRequiredService<IGit>();
        await git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        Func<Task<uint>> action = () => git.GetParentCount(classFixture.HeadSha, workingDir);

        (await action.Should().NotThrowAsync()).Which.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(GitTestFixture.TheoryData), MemberType = typeof(GitTestFixture))]
    public async Task InvalidSha_Throws(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices
            {
                MockGit = null,
                MockProcessOutFactory = null
            }).Build();

        string workingDir = Path.Combine(_tempDirectory, nameof(InvalidSha_Throws));
        IGit git = host.Services.GetRequiredService<IGit>();
        await git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        Func<Task<uint>> action = () => git.GetParentCount(classFixture.InvalidSha, workingDir);

        await action.Should().ThrowAsync<GitCommandException>();
    }

    [Theory]
    [MemberData(nameof(GitTestFixture.TheoryData), MemberType = typeof(GitTestFixture))]
    public async Task EmptyResult_ReturnsZero(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices
            {
                MockGit = null,
                MockProcessOutFactory = null
            }).Build();

        string workingDir = Path.Combine(_tempDirectory, nameof(EmptyResult_ReturnsZero));
        IGit git = host.Services.GetRequiredService<IGit>();
        await git.CloneRepoAsync(classFixture.CloneUrl, workingDir);

        Func<Task<uint>> action = () => git.GetParentCount(classFixture.BaseSha, workingDir);

        (await action.Should().NotThrowAsync()).Which.Should().Be(0);
    }
}