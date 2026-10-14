using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddAppConfig(IConfiguration configuration)
        {
            serviceCollection.AddOptionsWithValidateOnStart<Config>()
                .Bind(configuration.GetSection(nameof(Config)))
                .Validate(config => !string.IsNullOrWhiteSpace(config.IdRequestTokenEnvName)
                    && !string.IsNullOrWhiteSpace(config.SupabaseRequestTokenUrl)
                    && !string.IsNullOrWhiteSpace(config.IdRequestUrlEnvName),
                    "Config section missing or empty. Is appsettings.json being loaded?");
            
            return serviceCollection;
        }
        
        public IServiceCollection AddAppServices(WebhookEvent webhookEvent, ActionEventType eventType)
        {
            return serviceCollection
                .AddScoped<WebhookEvent>(_ => webhookEvent)
                .AddScoped(typeof(ActionEventType), _ => eventType)
                .AddGitHubClient()
                .AddScoped<IGit, Git>()
                .AddActionInfo()
                .AddScoped<ICommentBuilder, CommentBuilder>();
        }
        
        private IServiceCollection AddGitHubClient()
        {
            return serviceCollection
                .AddHttpClient<IGitHubClientFactory, GitHubClientFactory>()
                .Services
                .AddScoped<IProcessOutFactory, ProcessOutFactory>()
                .AddScoped<IGitHubClient>(provider =>
                {
                    IGitHubClientFactory factory = provider.GetRequiredService<IGitHubClientFactory>();

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