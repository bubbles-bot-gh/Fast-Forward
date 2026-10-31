using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.ActionInfo;

[Trait(Traits.Names.Category, Traits.Categories.Names.EventInfoTests)]
public sealed class UserHasPerms(EventInfoTestFixture classFixture) : IClassFixture<EventInfoTestFixture>
{
    [Fact]
    public async Task ValidPermissions_ReturnsTrue()
    {
        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        gitHubClientMock.Setup(m => m.IsCollaboratorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            classFixture.PullRequestOpenedEvent,
            ActionEventType.PullRequestOpened,
            new MockServices
            {
                MockActionOptions = classFixture.DefaultActionOptionsMock(),
                MockRepoInfo = classFixture.DefaultRepoInfoMock(),
                MockGitHubClient = gitHubClientMock,
                MockEventInfo = null
            }).Build();
        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();
        Func<Task<bool>> action = () => eventInfo.UserHasPerms;

        (await action.Should().NotThrowAsync()).Which.Should().BeTrue();
    }

    [Fact]
    public async Task InvalidPermissions_ReturnsFalse()
    {
        Mock<IGitHubClient> gitHubClientMock = new(MockBehavior.Strict);
        gitHubClientMock.Setup(m => m.IsCollaboratorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        IHost host = classFixture.CreateApplicationBuilderWithMocks(
            classFixture.PullRequestOpenedEvent,
            ActionEventType.PullRequestOpened,
            new MockServices
            {
                MockActionOptions = classFixture.DefaultActionOptionsMock(),
                MockRepoInfo = classFixture.DefaultRepoInfoMock(),
                MockGitHubClient = gitHubClientMock,
                MockEventInfo = null
            }).Build();
        IEventInfo eventInfo = host.Services.GetRequiredService<IEventInfo>();
        Func<Task<bool>> action = () => eventInfo.UserHasPerms;

        (await action.Should().NotThrowAsync()).Which.Should().BeFalse();
    }
}