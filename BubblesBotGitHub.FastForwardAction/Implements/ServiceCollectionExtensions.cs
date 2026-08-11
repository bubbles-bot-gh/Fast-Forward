using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.ActionInfo;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Implements;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddAppServices(WebhookEvent webhookEvent)
        {
            return serviceCollection
                .AddScoped<IActionOptions, ActionOptions>()
                .AddGitHubApiCaller()
                .AddActionInfo(webhookEvent);
        }
        
        private IServiceCollection AddGitHubApiCaller()
        {
            return serviceCollection
                .AddHttpClient<IGitHubApiCallerFactory, GitHubApiCallerFactory>()
                .Services
                .AddScoped<IGitHubApiCaller>(provider =>
                {
                    IGitHubApiCallerFactory factory = provider.GetRequiredService<IGitHubApiCallerFactory>();

                    return factory.Create();
                });
        }

        private IServiceCollection AddActionInfo(WebhookEvent webhookEvent)
        {
            return serviceCollection
                .AddScoped<WebhookEvent>(_ => webhookEvent)
                .AddScoped<IActionOptions, ActionOptions>()
                .AddScoped<IRepoInfo, RepoInfo>()
                .AddScoped<IEventInfo, EventInfo>()
                .AddScoped<IActionInfoFactory, ActionInfoFactory>()
                .AddScoped<IActionInfo>(provider =>
                {
                    IActionInfoFactory factory = provider.GetRequiredService<IActionInfoFactory>();
                    WebhookEvent gitHubEvent = provider.GetRequiredService<WebhookEvent>();
                    
                    return factory.Create(gitHubEvent);
                });
        }
    }
}