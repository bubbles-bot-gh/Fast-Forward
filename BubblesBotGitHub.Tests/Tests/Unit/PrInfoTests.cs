using BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;
using BubblesBotGitHub.Tests.Fixtures.PrInfoTests;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Tests.Unit;

[UsedImplicitly]
public sealed class PrInfoTests(PrInfoFixture classFixture) : IClassFixture<PrInfoFixture>
{
    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetBaseRefSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().BaseRef, (await subjectFactory()).BaseRef);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetBaseShaSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().BaseSha, (await subjectFactory()).BaseSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadRefSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadRef, (await subjectFactory()).HeadRef);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadShaSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadSha, (await subjectFactory()).HeadSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetHeadLabelSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().HeadLabel, (await subjectFactory()).HeadLabel);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetMergeBaseShaSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().MergeBaseSha, (await subjectFactory()).MergeBaseSha);
    }

    [Theory]
    [MemberData(nameof(PrInfoFixture.TheoryData), MemberType = typeof(PrInfoFixture))]
    public async Task GetMergeBaseParentsAmountSucceeds(Func<IPrInfo> expectedFactory, Func<Task<IPrInfo>> subjectFactory)
    {
        Assert.Equal(expectedFactory().MergeBaseParentsAmount, (await subjectFactory()).MergeBaseParentsAmount);
    }
}