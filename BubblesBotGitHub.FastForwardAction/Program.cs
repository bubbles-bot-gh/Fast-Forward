using BubblesBotGitHub.FastForward.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace BubblesBotGitHub.FastForward;

class Program
{
    static void Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAppServices();
        
        Console.WriteLine("Hello, World!");
    }
}