using System.Diagnostics.CodeAnalysis;
using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.Tests.Unit.Entities;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.ServiceCollectionExtensionsTests;

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
            mockServices: mockServices);
    }
}