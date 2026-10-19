using System.Text.Json;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.Models;

namespace BubblesBotGitHub.Tests.Tests.Unit;

[UsedImplicitly]
public class RepoInfoTests
{
    [Collection("Name")]
    public sealed class Name(RepoInfoFixture classFixture) : IClassFixture<RepoInfoFixture>
    {
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void ValidRepoName_ReturnsNonEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(webhookEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();

            actual.Name.Should().Be(webhookEvent.Repository.Name);
        }

        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void NullRepoName_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            JsonSerializerOptions opts = classFixture.CreateJsonSerializerOptionsWithRemovedField(typeof(Repository), "name");
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(newEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.Name.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void NullRepo_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            JsonSerializerOptions opts = classFixture.CreateJsonSerializerOptionsWithRemovedField(typeof(WebhookEvent), "repository");
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
            
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(newEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.Name.Should().BeEmpty();
        }
    }

    [Collection("Owner")]
    public sealed class Owner(RepoInfoFixture classFixture) : IClassFixture<RepoInfoFixture>
    {
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void ValidRepoOwner_ReturnsNonEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(webhookEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();

            actual.Owner.Should().Be(webhookEvent.Repository.Owner.Login);
        }
    
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void NullRepoOwner_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            JsonSerializerOptions opts = classFixture
                .CreateJsonSerializerOptionsWithRemovedField(typeof(User),"login");
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(newEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.Owner.Should().BeEmpty();
        }
        
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void NullRepo_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            JsonSerializerOptions opts = classFixture
                .CreateJsonSerializerOptionsWithRemovedField(typeof(WebhookEvent),"repository");
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(newEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.Owner.Should().BeEmpty();
        }
    }

    [Collection("CloneUrl")]
    public sealed class CloneUrl(RepoInfoFixture classFixture) : IClassFixture<RepoInfoFixture>
    {
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void ValidCloneUrl_ReturnsNonEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(webhookEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.CloneUrl.Should().Be(webhookEvent.Repository.CloneUrl);
        }
    
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void NullCloneUrl_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            JsonSerializerOptions opts = classFixture.CreateJsonSerializerOptionsWithRemovedField(typeof(Repository), "clone_url");
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(newEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.CloneUrl.Should().BeEmpty();
        }
        
        [Theory]
        [MemberData(nameof(RepoInfoFixture.TheoryData), MemberType = typeof(RepoInfoFixture))]
        public void NullRepo_ReturnsEmptyString(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            JsonSerializerOptions opts = classFixture
                .CreateJsonSerializerOptionsWithRemovedField(typeof(WebhookEvent),"repository");
            string json = JsonSerializer.Serialize(webhookEvent, webhookEvent.GetType());
            WebhookEvent newEvent = JsonSerializer.Deserialize(json, webhookEvent.GetType(), opts) as WebhookEvent
                ?? throw new InvalidOperationException($"Cannot deserialize event {json}");
        
            IRepoInfo actual = classFixture
                .CreateServicesWithMocks(newEvent, eventType)
                .BuildServiceProvider()
                .GetRequiredService<IRepoInfo>();
        
            actual.CloneUrl.Should().BeEmpty();
        }
    }
}