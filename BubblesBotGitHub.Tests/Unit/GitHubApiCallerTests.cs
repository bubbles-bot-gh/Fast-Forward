using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;
using Moq;
using Moq.Protected;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit;

public sealed class GitHubApiCallerTests
{
    public class GitHubApiCallerFactory(GitHubApiCallerFactoryFixture classFixture) 
        : IClassFixture<GitHubApiCallerFactoryFixture>
    {
        [Fact]
        public void CreatesSuccessfully()
        {
            Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, classFixture.RequestTokenEnvValue);
            Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, classFixture.RequestUrlEnvValue);

            classFixture.MockHttpHandler.Protected()
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

            classFixture.MockHttpHandler.Protected()
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
            
            IGitHubApiCallerFactory factory = classFixture.GetFactory(new HttpClient(classFixture.MockHttpHandler.Object));
            IGitHubApiCaller apiCaller = factory.Create();
            
            Assert.NotNull(apiCaller);
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


    public class GetPullRequest(GetPullRequestFixture classFixture) : IClassFixture<GetPullRequestFixture>
    {
        [Fact]
        public async Task SuccessfullyGetsPullRequest()
        {
            classFixture.MockOctokitClient
                .Setup(client => 
                    client.PullRequest.Get(
                        classFixture.Owner,
                        classFixture.Name,
                        (int)classFixture.PrNumber))
                .ReturnsAsync(new PullRequest());
            
            IGitHubApiCaller subject = classFixture.GetSubject(classFixture.MockOctokitClient.Object);
            PullRequest result = await subject
                .GetPullRequest(classFixture.Owner, classFixture.Name, classFixture.PrNumber);
            
            Assert.Equal((int)classFixture.PrNumber, result.Number);
        }

        [Fact]
        public async Task FailsToGetPullRequest()
        {
            classFixture.MockOctokitClient
                .Setup(client => 
                    client.PullRequest.Get(
                        classFixture.Owner,
                        classFixture.Name,
                        0))
                .ReturnsAsync(new PullRequest());
            
            IGitHubApiCaller subject = classFixture.GetSubject(classFixture.MockOctokitClient.Object);
            PullRequest result = await subject
                .GetPullRequest(classFixture.Owner, classFixture.Name, 0);
            
            Assert.Equal(0, result.Number);
        }
    }
}