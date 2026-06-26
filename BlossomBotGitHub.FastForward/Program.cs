using BlossomBotGitHub.FastForward.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace BlossomBotGitHub.FastForward;

class Program
{
    static void Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAppServices();
        
        Console.WriteLine("Hello, World!");
    }
}