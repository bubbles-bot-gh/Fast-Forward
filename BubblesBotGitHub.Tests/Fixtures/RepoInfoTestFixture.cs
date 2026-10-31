using System.Text.Json;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

public class RepoInfoTestFixture
{
    private static readonly PullRequestOpenedEvent PullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private static readonly IssueCommentCreatedEvent IssueCommentCreatedEvent = AssemblyFixture.GetIssueCommentCreatedEvent();
    private static readonly IssueCommentEditedEvent IssueCommentEditedEvent = AssemblyFixture.GetIssueCommentEditedEvent();

    public static TheoryData<WebhookEvent, ActionEventType> TheoryData => AssemblyFixture.TheoryData;

    public readonly Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder>
        CreateApplicationBuilderWithMocks = AssemblyFixture.CreateApplicationBuilderWithMocks;

    public IServiceCollection CreateServicesWithMocks(WebhookEvent webhookEvent, ActionEventType eventType)
    {
        return AssemblyFixture.CreateServiceCollectionWithMocks(webhookEvent, eventType, new MockServices { MockRepoInfo = null });
    }

    public readonly Func<Type, string, JsonSerializerOptions> CreateJsonSerializerOptionsWithRemovedField = 
        AssemblyFixture.CreateJsonSerializerOptionsWithRemovedField;
}