using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.Tests.Fixtures.PrInfoTests;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Tests;

[UsedImplicitly]
public sealed class PrInfoTests(PrInfoFixture classFixture) : IClassFixture<PrInfoFixture>
{
    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetBaseRefSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().BaseRef, subjectFactory().BaseRef);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetBaseShaSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().BaseSha, subjectFactory().BaseSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadRefSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadRef, subjectFactory().HeadRef);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadShaSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadSha, subjectFactory().HeadSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadLabelSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadLabel, subjectFactory().HeadLabel);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetMergeBaseShaSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().MergeBaseSha, subjectFactory().MergeBaseSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetMergeBaseParentsAmountSucceeds(Func<IPrInfo> expectedFactory, Func<IPrInfo> subjectFactory)
    {
        Assert.Equal(expectedFactory().MergeBaseParentsAmount, subjectFactory().MergeBaseParentsAmount);
    }
}