using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Git;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Entities;

public class MockServices
{
    public Mock<IGitHubApiCallerFactory>? MockGitHubApiCallerFactory = new();
    public Mock<IProcessOutFactory>? MockProcessOutFactory = new();
    public Mock<IGitHubApiCaller>? MockGitHubApiCaller = new();
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

    public static Mock<IGitHubApiCallerFactory> CreateMockGitHubApiCallerFactory()
    {
        Mock<IGitHubApiCallerFactory> mock = new();
        mock.Setup(factory => factory.Create())
            .Returns(Mock.Of<IGitHubApiCaller>());

        return mock;
    }
}