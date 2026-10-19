using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Repository = Octokit.Webhooks.Models.Repository;

namespace BubblesBotGitHub.Tests.Fixtures;

public class RepoInfoFixture
{
    private static readonly PullRequestOpenedEvent PullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    private static readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();
    
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData =>
    [
        new TheoryDataRow<WebhookEvent, ActionEventType>(PullRequestOpenedEvent, ActionEventType.PullRequestOpened),
        new TheoryDataRow<WebhookEvent, ActionEventType>(IssueCommentCreatedEvent, ActionEventType.IssueCommentCreated),
        new TheoryDataRow<WebhookEvent, ActionEventType>(IssueCommentEditedEvent, ActionEventType.IssueCommentEdited)
    ];

    public IServiceCollection CreateServicesWithMocks(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, new MockServices { MockRepoInfo = null });
    }

    public JsonSerializerOptions CreateJsonSerializerOptionsWithRemovedField(Type declaringType, string propertyName)
    {
        Action<JsonTypeInfo> typeInfoResolverModifier = (JsonTypeInfo typeInfo) =>
        {
            if (declaringType.IsAssignableFrom(typeInfo.Type))
            {
                // Remove property from deserialization contract
                JsonPropertyInfo? propertyInfo = typeInfo.Properties.Single(p => p.Name == propertyName);

                if (propertyInfo is not null)
                {
                    typeInfo.Properties.Remove(propertyInfo);
                }
            }
        };

        DefaultJsonTypeInfoResolver resolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { typeInfoResolverModifier }
        };

        return new JsonSerializerOptions { TypeInfoResolver = resolver };
    }
}