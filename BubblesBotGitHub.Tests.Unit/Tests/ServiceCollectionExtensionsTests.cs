using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.Tests.Unit.Entities;
using BubblesBotGitHub.Tests.Unit.Fixtures.ServiceCollectionExtensionsTests;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Tests;

[UsedImplicitly]
public sealed class ServiceCollectionExtensionsTests
{
    [Collection("GitHubApiCallerServiceTests")]
    public sealed class GitHubApiCallerService(ServiceCollectionExtensionsTestsFixture classFixture) 
        : IClassFixture<ServiceCollectionExtensionsTestsFixture>
    {
        [Fact]
        public void SucceedsWhenAddingFactoryService()
        {
            // Using mocked services, verify IActionInfo is registered as a service
            MockServices mockServices = new() { MockGitHubApiCallerFactory = null };
            IGitHubApiCallerFactory factory = classFixture
                .GetMockedServices(mockServices)
                .GetRequiredService<IGitHubApiCallerFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void SucceedsWhenAddingCallerService()
        {
            // Create mock factory that returns a mocked IActionInfo
            Mock<IGitHubApiCallerFactory> mockFactory = new();
            mockFactory
                .Setup(factory => factory.Create())
                .Returns(Mock.Of<IGitHubApiCaller>());

            // Using mocked services, verify IActionInfo is registered as a service
            // TODO: Move mock service usage to class fixture
            MockServices mockServices = new()
            {
                MockGitHubApiCaller = null, 
                MockGitHubApiCallerFactory = mockFactory
            };
            
            IGitHubApiCaller actual = classFixture
                .GetMockedServices(mockServices)
                .GetRequiredService<IGitHubApiCaller>();
            
            mockFactory.Verify(factory => factory.Create(), Times.Once);
            
            IGitHubApiCaller expected = mockFactory.Object.Create();
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
}