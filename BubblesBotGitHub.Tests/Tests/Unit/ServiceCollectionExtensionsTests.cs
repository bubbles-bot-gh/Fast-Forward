using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures.ServiceCollectionExtensionsTests;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace BubblesBotGitHub.Tests.Tests.Unit;

[UsedImplicitly]
public sealed class ServiceCollectionExtensionsTests
{
    [Collection("GitHubClientServiceTests")]
    public sealed class GitHubClientService(ServiceCollectionExtensionsTestsFixture classFixture) 
        : IClassFixture<ServiceCollectionExtensionsTestsFixture>
    {
        [Fact]
        public void SucceedsWhenAddingFactoryService()
        {
            // Using mocked services, verify IActionInfo is registered as a service
            MockServices mockServices = new() { MockGitHubClientFactory = null };
            IGitHubClientFactory factory = classFixture
                .GetMockedServices(mockServices)
                .GetRequiredService<IGitHubClientFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void SucceedsWhenAddingCallerService()
        {
            // Create mock factory that returns a mocked IActionInfo
            Mock<IGitHubClientFactory> mockFactory = new();
            mockFactory
                .Setup(factory => factory.Create())
                .Returns(Mock.Of<IGitHubClient>());

            // Using mocked services, verify IActionInfo is registered as a service
            // TODO: Move mock service usage to class fixture
            MockServices mockServices = new()
            {
                MockGitHubClient = null, 
                MockGitHubClientFactory = mockFactory
            };
            
            IGitHubClient actual = classFixture
                .GetMockedServices(mockServices)
                .GetRequiredService<IGitHubClient>();
            
            mockFactory.Verify(factory => factory.Create(), Times.Once);
            
            IGitHubClient expected = mockFactory.Object.Create();
            Assert.Same(expected, actual);
        }
    }
    
    [Collection("ActionInfoServiceTests")]
    public sealed class ActionInfoService(ServiceCollectionExtensionsTestsFixture classFixture) 
        : IClassFixture<ServiceCollectionExtensionsTestsFixture>
    {
        [Fact]
        public void SucceedsWhenAddingActionInfoFactory()
        {
            // Using mocked services, verify IActionInfo is registered as a service
            MockServices services = new() { MockActionInfoFactory = null };
            IActionInfoFactory actual = classFixture
                .GetMockedServices(services)
                .GetRequiredService<IActionInfoFactory>();
            
            Assert.NotNull(actual);
        }
            
        [Fact]
        public async Task SucceedsWhenAddingActionInfoService()
        {
            // Create mock factory that returns a mocked IActionInfo
            Mock<IActionInfoFactory> mockFactory = new();
            mockFactory
                .Setup(factory => factory.Create())
                .ReturnsAsync(Mock.Of<IActionInfo>());
            
            // Using mocked services, verify IActionInfo is registered as a service
            MockServices services = new()
            {
                MockActionInfo = null, 
                MockActionInfoFactory = mockFactory
            };
            IActionInfo actual = classFixture
                .GetMockedServices(services)
                .GetRequiredService<IActionInfo>();
            
            mockFactory.Verify(factory => factory.Create(), Times.Once);
            
            IActionInfo expected = await mockFactory.Object.Create();
            Assert.Same(expected, actual);
        }
    }

    [Collection("AppConfigTests")]
    public sealed class AppConfigService(ITestOutputHelper outputHelper,
        ServiceCollectionExtensionsTestsFixture classFixture)
        : IClassFixture<ServiceCollectionExtensionsTestsFixture>
    {
        [Theory]
        [MemberData(nameof(ServiceCollectionExtensionsTestsFixture.TheoryData),
            MemberType = typeof(ServiceCollectionExtensionsTestsFixture))]
        public void OptionMissing_FailsValidation(string optionName)
        {
            IServiceProvider provider = classFixture.CreateServiceProviderWithOptionMissing(optionName);
            Action action = () => _ = provider.GetRequiredService<IOptions<Config>>().Value;

            action.Should().Throw<OptionsValidationException>();
        }
    }
}