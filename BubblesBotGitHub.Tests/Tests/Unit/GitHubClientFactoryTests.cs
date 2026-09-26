using System.Net;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;
using Moq;
using Moq.Protected;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public class GitHubClientFactoryTests(GitHubClientFactoryFixture classFixture) 
    : IClassFixture<GitHubClientFactoryFixture>
{
    [Fact]
    public void SucceedsCreatingInstance()
    {
        Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, classFixture.RequestTokenEnvValue);
        Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, classFixture.RequestUrlEnvValue);
        
        // Mock setup
        classFixture
            .MockHttpHandler
            .Protected()
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

        classFixture
            .MockHttpHandler
            .Protected()
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
        
        // Get subject with mocked object
        IGitHubClientFactory factory = classFixture.GetFactory(new HttpClient(classFixture.MockHttpHandler.Object));
        IGitHubClient client = factory.Create();
        
        // Verify results
        Assert.NotNull(client);
        classFixture.MockHttpHandler.Protected().Verify(
            "Send",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.RequestUri!.Host.Contains(classFixture.GitHubUserContentHost)),
            ItExpr.IsAny<CancellationToken>());
        
        classFixture.MockHttpHandler.Protected().Verify(
            "Send",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.RequestUri!.Host.Contains(classFixture.SupabaseHost)),
            ItExpr.IsAny<CancellationToken>());
    }
}