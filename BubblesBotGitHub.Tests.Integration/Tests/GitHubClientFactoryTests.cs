using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using BubblesBotGitHub.Tests.Integration.Fixtures.GitHubApiCallerFactoryTests;
using JetBrains.Annotations;
using Moq;
using Moq.Protected;

namespace BubblesBotGitHub.Tests.Integration.Tests;

[UsedImplicitly]
[Collection("GitHubApiCallerFactoryIntegrationTests")]
public sealed class GitHubClientFactoryTests(GitHubApiCallerFactoryFixture classFixture) : IClassFixture<GitHubApiCallerFactoryFixture>, IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, classFixture.RequestToken);
        Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, classFixture.RequestUrl);
            
        return ValueTask.CompletedTask;
    }
        
    public ValueTask DisposeAsync()
    {
        Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, null);
        Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, null);
            
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task SuccessfullyExchangesOidcAndInstallationTokens()
    {
        Mock<HttpMessageHandler> reqHandlerMock = new(MockBehavior.Strict);
        
        // Mock of the POST request to the GitHub ACTIONS_ID_TOKEN_REQUEST_URL
        HttpRequestMessage firstReq = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(classFixture.RequestUrl),
            Content = new StringContent($"\"value\": \"{classFixture.RequestToken}\""),
            Headers =
            {
                { "ContentType", "application/vnd.github.v3+json" }
            }
        };
        HttpResponseMessage firstRes = new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(classFixture.RequestToken)
        };
        
        reqHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                Times.Once(),
                firstReq, 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(firstRes);
     
        // Mock of the POST request to the auth uri
        HttpRequestMessage secondReq = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri (classFixture.RequestUrl),
            Content = new StringContent(await firstRes.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
        };
        
        HttpClient httpClient = new(reqHandlerMock.Object);
        
        GitHubClientFactory factory = new(httpClient);
        IGitHubClient caller = factory.Create();

        Assert.NotNull(caller);
    }

    [Fact]
    public void ThrowsExceptionWhenIdTokenIsNotSet()
    {
        Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, null);
        Mock<HttpClient> httpClientMock = new();
        httpClientMock.VerifyNoOtherCalls();
        GitHubClientFactory factory = new(httpClientMock.Object);
        
        Assert.Throws<InvalidOperationException>(factory.Create);
    }

    [Fact]
    public void ThrowsWhenUrlIsNotSet()
    {
        Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, null);
        Mock<HttpClient> httpClientMock = new();
        httpClientMock.VerifyNoOtherCalls();
        GitHubClientFactory factory = new(httpClientMock.Object);
        
        Assert.Throws<InvalidOperationException>(factory.Create);
    }
}