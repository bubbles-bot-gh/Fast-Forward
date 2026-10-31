using System.Text.Json;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Octokit.Webhooks.Models;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Trait(Traits.Names.Category, Traits.Categories.Names.RepoInfoTests)]
public sealed class CloneUrl(RepoInfoTestFixture classFixture) : IClassFixture<RepoInfoTestFixture>
{
    [Theory]
    [MemberData(nameof(RepoInfoTestFixture.TheoryData), MemberType = typeof(RepoInfoTestFixture))]
    public void ValidRepoCloneUrl_ReturnsNonEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            webhookEvent,
            eventType,
            new MockServices
            {
                MockRepoInfo = null
            }).Build();
        IRepoInfo repoInfo = host.Services.GetRequiredService<IRepoInfo>();

        repoInfo.CloneUrl.Should().NotBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(RepoInfoTestFixture.TheoryData), MemberType = typeof(RepoInfoTestFixture))]
    public void NullRepoCloneUrl_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        JsonSerializerOptions opts = classFixture.CreateJsonSerializerOptionsWithRemovedField(typeof(Repository), "clone_url");
        string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
        WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
            ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            newEvent,
            eventType,
            new MockServices
            {
                MockRepoInfo = null
            }).Build();
        IRepoInfo repoInfo = host.Services.GetRequiredService<IRepoInfo>();

        repoInfo.CloneUrl.Should().BeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(RepoInfoTestFixture.TheoryData), MemberType = typeof(RepoInfoTestFixture))]
    public void NullRepo_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        JsonSerializerOptions opts = classFixture.CreateJsonSerializerOptionsWithRemovedField(typeof(WebhookEvent), "repository");
        string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
        WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
            ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            newEvent,
            eventType,
            new MockServices
            {
                MockRepoInfo = null
            }).Build();
        IRepoInfo repoInfo = host.Services.GetRequiredService<IRepoInfo>();

        repoInfo.CloneUrl.Should().BeEmpty();
    }
}