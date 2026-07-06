using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Implements.Git;
using BubblesBotGitHub.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(AssemblyFixture))]
namespace BubblesBotGitHub.Tests.Fixtures;

public class AssemblyFixture
{
    private static readonly string RootWorkingDir = "./tmp";
    public IServiceProvider Services { get; }
    public static readonly string BaseSha = "a3f4edfee60026fc44989822ac8789e376f374a2";
    public static readonly string HeadSha = "1f85b89057373f54de739944889d1abec8c048b0";
    public static readonly string GitTestsDir = $"{RootWorkingDir}/GitTest";
    public static string RepoUrl => "https://github.com/LuneiSolei/Fast-Forward-Blossom-Bot-Tests.git";

    public AssemblyFixture()
    {
        // Create service collection
        IServiceCollection serviceCollection = new ServiceCollection();
        
        // Add services
        Services = serviceCollection.AddScoped<IProcessOutFactory, ProcessOutFactory>()
            .AddScoped<IGit, Git>(provider =>
        {
            IProcessOutFactory factory = provider.GetService<IProcessOutFactory>()
                                         ?? throw new NullReferenceException("ServiceProvider.ProcessOutFactory");
            Git git = new Git(factory);
            
            return git;
        })
            .BuildServiceProvider();
    }
}