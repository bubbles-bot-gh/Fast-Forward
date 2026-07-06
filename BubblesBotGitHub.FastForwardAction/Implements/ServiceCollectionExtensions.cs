using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using Microsoft.Extensions.DependencyInjection;

namespace BubblesBotGitHub.FastForward.Implements;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        private IServiceCollection AddGitHubApiCaller()
        {
            return serviceCollection
                .AddScoped<IGitHubApiCallerFactory, GitHubApiCallerFactory>()
                .AddScoped<IGitHubApiCaller>(provider =>
            {
                IGitHubApiCallerFactory factory = provider.GetRequiredService<IGitHubApiCallerFactory>();
                
                return factory.Create();
            });
        }
        
        public IServiceCollection AddAppServices()
        {
            return serviceCollection
                .AddScoped<IActionOptions, ActionOptions>()
                .AddGitHubApiCaller();
        }
    }
}