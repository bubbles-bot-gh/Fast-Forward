using BubblesBotGitHub.FastForward;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
namespace BubblesBotGitHub.Tests.Unit;

[Collection(Collections.Names.ProgramTests)]
[Trait(Traits.Names.Category, Traits.Categories.Names.ProgramTests)]
public sealed class FastForwardRunner(ProgramTestFixture classFixture, ITestOutputHelper outputHelper)
    : IClassFixture<ProgramTestFixture>
{
    [Theory]
    [MemberData(
        nameof(ProgramTestFixture.TheoryDataFastForwardRunner),
        MemberType = typeof(ProgramTestFixture),
        DisableDiscoveryEnumeration = true)
    ]
    public async Task WhenSetTo(
        string eventPath,
        string eventName,
        bool commandInvoked,
        bool isAutoMerge,
        bool shouldExit,
        string postComment,
        Times commentBuildCalls,
        Times fastForwardAsyncCalls,
        Times postCommentAsyncCalls,
        int exitCode)
    {
        using IHost host = await classFixture.BuildHost(
            eventPath,
            eventName,
            commandInvoked,
            isAutoMerge,
            postComment,
            shouldExit);
        Func<Task<int>> action = () => Program.FastForwardRunner(host);

        (await action.Should().NotThrowAsync()).Which.Should().Be(exitCode);
        ProgramTestFixture.VerifyCommentBuildCalled(host, commentBuildCalls);
        ProgramTestFixture.VerifyFastForwardAsyncCalled(host, fastForwardAsyncCalls);
        ProgramTestFixture.VerifyPostCommentAsyncCalled(host, postCommentAsyncCalls);
    }
}