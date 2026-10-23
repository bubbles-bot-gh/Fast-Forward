using System.Diagnostics.CodeAnalysis;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Extensions;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BubblesBotGitHub.Tests.Fixtures.ServiceCollectionExtensionsTests;

[
    UsedImplicitly,
    SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
    SuppressMessage("Performance", "CA1822:Mark members as static")
]
public class ServiceCollectionExtensionsTestsFixture : IClassFixture<ServiceCollectionExtensionsTestsFixture>
{
    public static TheoryData<string> TheoryData =>
    [
        new TheoryDataRow<string>("IdRequestTokenEnvName"),
        new TheoryDataRow<string>("IdRequestUrlEnvName"),
        new TheoryDataRow<string>("SupabaseRequestTokenUrl"),
        new TheoryDataRow<string>("SupabasePublishableKey"),
        new TheoryDataRow<string>("Audience"),
        new TheoryDataRow<string>("WorkingDir")
    ];
    
    public IServiceProvider GetMockedServices(MockServices mockServices)
    {
        return AssemblyFixture.CreateServiceCollectionWithMocks(
            webhookEvent: AssemblyFixture.GetPullRequestOpenedEvent(),
            eventType: ActionEventType.PullRequestOpened,
            mockServices: mockServices).BuildServiceProvider();
    }

    public IHostApplicationBuilder ConfigureAppHost()
    {
        IHostApplicationBuilder host = new HostApplicationBuilder();
        host.Configuration.Sources.Clear();
        host.Configuration.AddJsonFile("appsettings.test.json", optional: false);
        host.Services.AddAppConfig(host.Configuration);
        host.Services.AddAppServices(AssemblyFixture.GetPullRequestOpenedEvent(), ActionEventType.PullRequestOpened);
        
        return host;
    }

    public IServiceProvider CreateServiceProviderWithOptionMissing(string optionName)
    {
        IHostApplicationBuilder host = ConfigureAppHost();
        Dictionary<string, string?> overrides = new()
        {
            [$"Config:{optionName}"] = " "
        };
        host.Configuration.AddInMemoryCollection(overrides);
        
        return host.Services.BuildServiceProvider();
    }
}