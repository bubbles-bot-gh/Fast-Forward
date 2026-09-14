using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.ActionInfo;
using BubblesBotGitHub.FastForward.Implements.Git;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Implements;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddAppServices(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            return serviceCollection
                .AddScoped<WebhookEvent>(_ => webhookEvent)
                .AddScoped(typeof(ActionEventType), _ => eventType)
                .AddGitHubApiCaller()
                .AddScoped<IGit, Git.Git>()
                .AddActionInfo();
        }
        
        private IServiceCollection AddGitHubApiCaller()
        {
            return serviceCollection
                .AddHttpClient<IGitHubApiCallerFactory, GitHubApiCallerFactory>()
                .Services
                .AddScoped<IProcessOutFactory, ProcessOutFactory>()
                .AddScoped<IGitHubApiCaller>(provider =>
                {
                    IGitHubApiCallerFactory factory = provider.GetRequiredService<IGitHubApiCallerFactory>();

                    return factory.Create();
                });
        }

        private IServiceCollection AddActionInfo()
        {
            return serviceCollection
                .AddScoped<IActionOptions, ActionOptions>()
                .AddScoped<IRepoInfo, RepoInfo>()
                .AddScoped<IEventInfo, EventInfo>()
                .AddScoped<IPrInfo, PrInfo>()
                .AddScoped<IActionInfoFactory, ActionInfoFactory>()
                .AddScoped<IActionInfo>(provider =>
                {
                    IActionInfoFactory factory = provider.GetRequiredService<IActionInfoFactory>();
                    
                    return factory.Create().Result;
                });
        }
    }
}