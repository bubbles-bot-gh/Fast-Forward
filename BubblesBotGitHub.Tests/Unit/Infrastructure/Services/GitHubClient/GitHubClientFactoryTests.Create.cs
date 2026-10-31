using System.Net;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace BubblesBotGitHub.Tests.Unit.Infrastructure.Services.GitHubClient;

[Trait(Traits.Names.Category, Traits.Categories.Names.GitHubClientFactoryTests)]
public sealed class Create(GitHubClientFactoryTestFixture classFixture) : IClassFixture<GitHubClientFactoryTestFixture>
{
    [Fact]
    public void ValidArgs_CreateInstance()
    {
        IOptions<Config> options = classFixture.CreateDefaultTestOptions();
        classFixture.SetDefaultEnvVars();
            
        Mock<HttpMessageHandler> httpHandlerMock = new(MockBehavior.Strict);
        classFixture.SetupIdTokenResponse(httpHandlerMock, new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(classFixture.IdTokenResponseMock)
        });
        classFixture.SetupInstallationTokenResponse(httpHandlerMock, new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(classFixture.InstallationTokenResponseMock)
        });
            
        IGitHubClient client = classFixture.BuildGitHubClient(options, new HttpClient(httpHandlerMock.Object));

        client.Should().NotBeNull();
        classFixture.VerifyIdTokenResponseCalled(httpHandlerMock);
        classFixture.VerifyInstallationTokenResponseCalled(httpHandlerMock);

        classFixture.ResetEnvVars();
    }
    
    [Fact]
    public void IdRequestTokenUrlNotSet_Throws()
        {
            IOptions<Config> options = classFixture.CreateDefaultTestOptions();
            Environment.SetEnvironmentVariable(options.Value.IdRequestTokenEnvName, null);

            Mock<HttpMessageHandler> httpHandlerMock = new(MockBehavior.Strict);
            Action action = () => _ = classFixture.BuildGitHubClient(options, new HttpClient(httpHandlerMock.Object));

            action.Should().Throw<InvalidOperationException>().WithMessage("ID token not set");
            httpHandlerMock.Protected().Verify("Send", 
                Times.Never(), 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

    [Fact]
    public void IdRequestUrlNotSet_Throws()
        {
            IOptions<Config> options = classFixture.CreateDefaultTestOptions();
            Environment.SetEnvironmentVariable(options.Value.IdRequestTokenEnvName, classFixture.IdRequestToken);
            
            Mock<HttpMessageHandler> httpHandlerMock = new(MockBehavior.Strict);
            Action action = () => _ = classFixture.BuildGitHubClient(options, new HttpClient(httpHandlerMock.Object));

            action.Should().Throw<InvalidOperationException>().WithMessage("URL not set");
            httpHandlerMock.Protected()
                .Verify("Send", 
                    Times.Never(), 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>());
            
            classFixture.ResetEnvVars();
        }

    [Fact]
    public void IdTokenValueFieldNull_Throws()
        {
            IOptions<Config> options = classFixture.CreateDefaultTestOptions();
            classFixture.SetDefaultEnvVars();

            Mock<HttpMessageHandler> httpHandlerMock = new(MockBehavior.Strict);
            classFixture.SetupIdTokenResponse(httpHandlerMock, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"value": null}""")
            });
            Action action = () => _ = classFixture.BuildGitHubClient(options, new HttpClient(httpHandlerMock.Object));

            action.Should().Throw<InvalidOperationException>().WithMessage("OIDC response 'value' field is null");
            classFixture.VerifyIdTokenResponseCalled(httpHandlerMock);

            classFixture.ResetEnvVars();
        }

    [Fact]
    public void InstallationTokenValueFieldNull_Throws()
        {
            IOptions<Config> options = classFixture.CreateDefaultTestOptions();
            classFixture.SetDefaultEnvVars();

            Mock<HttpMessageHandler> httpHandlerMock = new(MockBehavior.Strict);
            classFixture.SetupIdTokenResponse(httpHandlerMock, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(classFixture.IdTokenResponseMock)
            });
            classFixture.SetupInstallationTokenResponse(httpHandlerMock, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"token": null}""")
            });

            Action action = () => _ = classFixture.BuildGitHubClient(options, new HttpClient(httpHandlerMock.Object));

            action.Should().Throw<InvalidOperationException>().WithMessage("Installation token response 'token' field is null");
            classFixture.VerifyIdTokenResponseCalled(httpHandlerMock);
            classFixture.VerifyInstallationTokenResponseCalled(httpHandlerMock);

            classFixture.ResetEnvVars();
        }
}