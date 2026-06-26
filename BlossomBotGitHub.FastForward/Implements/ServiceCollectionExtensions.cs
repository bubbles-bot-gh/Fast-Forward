using BlossomBotGitHub.FastForward.Core.ActionInfo;
using Microsoft.Extensions.DependencyInjection;

namespace BlossomBotGitHub.FastForward.Implements;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        private IServiceCollection AddActionOptions()
        {
            serviceCollection.AddScoped<IActionOptions, ActionOptions>();
            
            return serviceCollection;
        }
        
        public IServiceCollection AddAppServices()
        {
            IServiceCollection services = serviceCollection
                .AddActionOptions();

            return serviceCollection;
        }
    }
}