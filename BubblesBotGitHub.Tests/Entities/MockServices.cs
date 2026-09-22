using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Moq;

namespace BubblesBotGitHub.Tests.Entities;

public class MockServices
{
    public Mock<IGitHubClientFactory>? MockGitHubApiCallerFactory = new();
    public Mock<IProcessOutFactory>? MockProcessOutFactory = new();
    public Mock<IGitHubClient>? MockGitHubApiCaller = new();
    public Mock<IGit>? MockGit = new();
    public Mock<IActionOptions>? MockActionOptions = new();
    public Mock<IEventInfo>? MockEventInfo = new();
    public Mock<IRepoInfo>? MockRepoInfo = new();
    public Mock<IPrInfo>? MockPrInfo = new();
    public Mock<IActionInfoFactory>? MockActionInfoFactory = new();
    public Mock<IActionInfo>? MockActionInfo = new();
    
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

    public static Mock<IGitHubClientFactory> CreateMockGitHubApiCallerFactory()
    {
        Mock<IGitHubClientFactory> mock = new();
        mock.Setup(factory => factory.Create())
            .Returns(Mock.Of<IGitHubClient>());

        return mock;
    }
}