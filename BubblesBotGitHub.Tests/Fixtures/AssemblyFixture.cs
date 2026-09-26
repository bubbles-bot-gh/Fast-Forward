using System.Text.Json;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Octokit;
using Octokit.Internal;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

[assembly: AssemblyFixture(typeof(AssemblyFixture))]
namespace BubblesBotGitHub.Tests.Fixtures;

public class AssemblyFixture : IAsyncLifetime
{
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        Directory.Delete(RootWorkingDir, true);
        
        return ValueTask.CompletedTask;        
    }
    
    public const string RootWorkingDir = "./tmp";
    public const string CustomCommand = "/fast-forward";
    private const string PrOpenedEventJson = "./Events/PullRequestOpenedEvent.json";
    private const string IssueCommentCreatedEventJson = "./Events/IssueCommentCreatedEvent.json";
    private const string IssueCommentEditedEventJson = "./Events/IssueCommentEditedEvent.json";
    
    public const string TestRepoUrl = "https://github.com/LuneiSolei/Fast-Forward-Blossom-Bot-Tests.git";
    public const string TestRepoBaseSha = "a3f4edfee60026fc44989822ac8789e376f374a2";
    public const string TestRepoHeadSha = "1f85b89057373f54de739944889d1abec8c048b0";

    internal static GitHubCommit GetGitHubCommit()
    {
        SimpleJsonSerializer serializer = new();
        return serializer.Deserialize<GitHubCommit>(File.ReadAllText("./Events/Commit.json"));
    }
    
    internal static IServiceProvider CreateServiceCollectionWithMocks(
        WebhookEvent webhookEvent,
        ActionEventType eventType,
        MockServices mockServices)
    {
        IServiceCollection services = new ServiceCollection().AddAppServices(webhookEvent, eventType);

        // Replace IGitHubClientFactory service, if needed
        if (mockServices.MockGitHubClientFactory is not null)
            ReplaceService(services, mockServices.MockGitHubClientFactory);

        // Replace IProcessOutFactory service, if needed
        if (mockServices.MockProcessOutFactory is not null)
            ReplaceService(services, mockServices.MockProcessOutFactory);
            
        // Replace IGitHubClient service, if needed
        if (mockServices.MockGitHubClient is not null)
            ReplaceService(services, mockServices.MockGitHubClient);

        // Replace IGit service, if needed
        if (mockServices.MockGit is not null)
            ReplaceService(services, mockServices.MockGit);
        
        // Replace IActionOptions service, if needed
        if (mockServices.MockActionOptions is not null)
            ReplaceService(services, mockServices.MockActionOptions);
        
        // Replace IRepoInfo service, if needed
        if (mockServices.MockRepoInfo is not null)
            ReplaceService(services, mockServices.MockRepoInfo);
        
        // Replace IEventInfo service, if needed
        if (mockServices.MockEventInfo is not null)
            ReplaceService(services, mockServices.MockEventInfo);
        
        // Replace IPrInfo service, if needed
        if (mockServices.MockPrInfo is not null)
            ReplaceService(services, mockServices.MockPrInfo);

        // Replace IActionInfoFactory service, if needed
        if (mockServices.MockActionInfoFactory is not null)
            ReplaceService(services, mockServices.MockActionInfoFactory);

        // Replace IActionInfo service, if needed
        if (mockServices.MockActionInfo is not null)
            ReplaceService(services, mockServices.MockActionInfo);
        
        // Replace HttpClient, if needed
        if (mockServices.MockHttpClient is not null)
            ReplaceService(services, mockServices.MockHttpClient);

        return services.BuildServiceProvider();
    }

    private static void ReplaceService<TService>(IServiceCollection services, Mock<TService> mockService) 
        where TService : class
    {
        TService ScopedFunc(IServiceProvider _) => mockService.Object;
        ServiceDescriptor descriptor = ServiceDescriptor.Scoped(ScopedFunc);
        services.Replace(descriptor);
    }

    internal static PullRequestOpenedEvent GetPullRequestOpenedEvent()
    {
        string file = File.ReadAllText(PrOpenedEventJson);
        PullRequestOpenedEvent eventData = JsonSerializer.Deserialize<PullRequestOpenedEvent>(file) 
            ?? throw new InvalidOperationException();
        
        return eventData;
    }

    internal static IssueCommentCreatedEvent GetIssueCommentCreatedEvent()
    {
        string file = File.ReadAllText(IssueCommentCreatedEventJson);
        IssueCommentCreatedEvent eventData = JsonSerializer.Deserialize<IssueCommentCreatedEvent>(file)
            ?? throw new InvalidOperationException();

        return eventData;
    }

    internal static IssueCommentEditedEvent GetIssueCommentEditedEvent()
    {
        string file = File.ReadAllText(IssueCommentEditedEventJson);
        IssueCommentEditedEvent eventData = JsonSerializer.Deserialize<IssueCommentEditedEvent>(file)
            ?? throw new InvalidOperationException();

        return eventData;
    }
}