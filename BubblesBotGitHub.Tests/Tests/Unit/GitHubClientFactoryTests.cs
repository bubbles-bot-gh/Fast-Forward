using System.Net;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace BubblesBotGitHub.Tests.Tests.Unit;

public class GitHubClientFactoryTests
{
    public class Create(GitHubClientFactoryFixture classFixture) : IClassFixture<GitHubClientFactoryFixture>
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
            
            IGitHubClientFactory subject = classFixture.GetSubject(options, new HttpClient(httpHandlerMock.Object));
            IGitHubClient client = subject.Create();

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
            IGitHubClientFactory subject = classFixture.GetSubject(options, new HttpClient(httpHandlerMock.Object));
            Action action = () => subject.Create();

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
            IGitHubClientFactory subject = classFixture.GetSubject(options, new HttpClient(httpHandlerMock.Object));
            Action action = () => subject.Create();

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

            IGitHubClientFactory subject = classFixture.GetSubject(options, new HttpClient(httpHandlerMock.Object));
            Action action = () => subject.Create();

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

            IGitHubClientFactory subject = classFixture.GetSubject(options, new HttpClient(httpHandlerMock.Object));
            Action action = () => subject.Create();

            action.Should().Throw<InvalidOperationException>().WithMessage("Installation token response 'token' field is null");
            classFixture.VerifyIdTokenResponseCalled(httpHandlerMock);
            classFixture.VerifyInstallationTokenResponseCalled(httpHandlerMock);

            classFixture.ResetEnvVars();
        }
    }
}