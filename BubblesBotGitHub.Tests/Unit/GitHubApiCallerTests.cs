using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;
using Moq;
using Moq.Protected;

namespace BubblesBotGitHub.Tests.Unit;

public sealed class GitHubApiCallerTests(GitHubApiCallerFixture classFixture) 
    : IClassFixture<GitHubApiCallerFixture>
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler = new(MockBehavior.Strict);

    [Fact]
    public void CreateApiCaller()
    {
        Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, classFixture.RequestTokenEnvValue);
        Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, classFixture.RequestUrlEnvValue);

        _mockHttpHandler.Protected()
            .Setup<HttpResponseMessage>(
                "Send",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get 
                    && req.RequestUri!.Host.Contains(classFixture.GitHubUserContentHost)
                ),
                ItExpr.IsAny<CancellationToken>())
            .Returns(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(classFixture.MockOidcValue)
                });

        _mockHttpHandler.Protected()
            .Setup<HttpResponseMessage>(
                "Send",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                    && req.RequestUri!.Host.Contains(classFixture.SupabaseHost)
                ),
                ItExpr.IsAny<CancellationToken>())
            .Returns(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(classFixture.MockInstallationTokenValue)
                });
        
        HttpClient client = new(_mockHttpHandler.Object);
        IGitHubApiCallerFactory factory = new GitHubApiCallerFactory(client);
        IGitHubApiCaller apiCaller = factory.Create();
        
        Assert.NotNull(apiCaller);
        _mockHttpHandler.Protected().Verify(
            "Send",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.RequestUri!.Host.Contains(classFixture.GitHubUserContentHost)),
            ItExpr.IsAny<CancellationToken>());
        
        _mockHttpHandler.Protected().Verify(
            "Send",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.RequestUri!.Host.Contains(classFixture.SupabaseHost)),
            ItExpr.IsAny<CancellationToken>());
    }
}