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
}