using BubblesBotGitHub.FastForward.Application;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using Moq;

namespace BubblesBotGitHub.Tests.Entities;

public class MockServices
{
    public Mock<IGitHubClientFactory>? MockGitHubClientFactory = new(MockBehavior.Loose);
    public Mock<IProcessOutFactory>? MockProcessOutFactory = new(MockBehavior.Loose);
    public Mock<IGitHubClient>? MockGitHubClient = new(MockBehavior.Loose);
    public Mock<IGit>? MockGit = new(MockBehavior.Loose);
    public Mock<IActionOptions>? MockActionOptions = new(MockBehavior.Loose);
    public Mock<IEventInfo>? MockEventInfo = new(MockBehavior.Loose);
    public Mock<IRepoInfo>? MockRepoInfo = new(MockBehavior.Loose);
    public Mock<IPrInfo>? MockPrInfo = new(MockBehavior.Loose);
    public Mock<IActionInfoFactory>? MockActionInfoFactory = new(MockBehavior.Loose);
    public Mock<IActionInfo>? MockActionInfo = new(MockBehavior.Loose);
    public Mock<HttpClient>? MockHttpClient = new(MockBehavior.Loose);
    public Mock<ICommentBuilder>? MockCommentBuilder = new(MockBehavior.Loose);
}