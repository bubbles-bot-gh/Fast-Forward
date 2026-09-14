using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.Tests.Unit.Fixtures;

namespace BubblesBotGitHub.Tests.Unit.Tests;

public sealed class EventInfoTests(EventInfoFixture classFixture) : IClassFixture<EventInfoFixture>
{
    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public void GetCommentBodySucceeds(IEventInfo expected, IEventInfo actual)
    {
        Assert.Equal(expected.CommentBody, actual.CommentBody);
    }
}