using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.Tests.Fixtures;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public sealed class EventInfoTests(EventInfoFixture classFixture) : IClassFixture<EventInfoFixture>
{
    [Theory]
    [MemberData(nameof(EventInfoFixture.TheoryData), MemberType = typeof(EventInfoFixture))]
    public void GetCommentBodySucceeds(IEventInfo expected, IEventInfo actual)
    {
        Assert.Equal(expected.CommentBody, actual.CommentBody);
    }
}