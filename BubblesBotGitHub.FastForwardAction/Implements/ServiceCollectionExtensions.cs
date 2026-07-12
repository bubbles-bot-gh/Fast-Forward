using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using Microsoft.Extensions.DependencyInjection;

namespace BubblesBotGitHub.FastForward.Implements;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddAppServices()
        {
            return serviceCollection
                .AddScoped<IActionOptions, ActionOptions>()
                .AddGitHubApiCaller();
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
    }
}