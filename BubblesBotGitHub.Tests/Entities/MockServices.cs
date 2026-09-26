using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using Moq;

namespace BubblesBotGitHub.Tests.Entities;

public class MockServices
{
    public Mock<IGitHubClientFactory>? MockGitHubClientFactory = new();
    public Mock<IProcessOutFactory>? MockProcessOutFactory = new();
    public Mock<IGitHubClient>? MockGitHubClient = new();
    public Mock<IGit>? MockGit = new();
    public Mock<IActionOptions>? MockActionOptions = new();
    public Mock<IEventInfo>? MockEventInfo = new();
    public Mock<IRepoInfo>? MockRepoInfo = new();
    public Mock<IPrInfo>? MockPrInfo = new();
    public Mock<IActionInfoFactory>? MockActionInfoFactory = new();
    public Mock<IActionInfo>? MockActionInfo = new();
    public Mock<HttpClient>? MockHttpClient = new();
    
    public static Mock<IRepoInfo> CreateMockRepoInfo(string owner, string cloneUrl, string name)
    {
        Mock<IRepoInfo> mock = new();
        mock.SetupGet(repoInfo => repoInfo.Owner)
            .Returns(owner);

        mock.SetupGet(repoInfo => repoInfo.CloneUrl)
            .Returns(cloneUrl);

        mock.SetupGet(repoInfo => repoInfo.Name)
            .Returns(name);
        
        return mock;
    }

    public static Mock<IGitHubClientFactory> CreateMockGitHubClientFactory()
    {
        Mock<IGitHubClientFactory> mock = new();
        mock.Setup(factory => factory.Create())
            .Returns(Mock.Of<IGitHubClient>());

        return mock;
    }
}