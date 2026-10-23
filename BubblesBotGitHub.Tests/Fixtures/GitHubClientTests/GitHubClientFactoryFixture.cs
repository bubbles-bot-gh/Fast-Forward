using System.Linq.Expressions;
using System.Net;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;

[UsedImplicitly]
public sealed class GitHubClientFactoryFixture
{
    public readonly string IdRequestTokenUrl = "http://localhost/idtoken?api-version=2.0";
    public readonly string SupabaseUrl = "http://localhost/supabase";
    public readonly string IdTokenResponseMock = """{"value":"fake-oidc-123"}""";
    public readonly string InstallationTokenResponseMock = """{"token":"fake-installation-token-123"}""";
    public readonly string IdRequestToken = "some-valid-token";
    private readonly string IdRequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN";
    private readonly string IdRequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL";
    private readonly Expression _idTokenResponseExpr;
    private readonly Expression _installationTokenResponseExpr;

    public GitHubClientFactoryFixture()
    {
        _idTokenResponseExpr = ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get 
            && req.RequestUri!.AbsoluteUri.StartsWith(IdRequestTokenUrl));
        
        _installationTokenResponseExpr = ItExpr.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post
            && req.RequestUri!.AbsoluteUri.StartsWith(SupabaseUrl));
    }

    internal IOptions<Config> CreateDefaultTestOptions()
    {
        return Options.Create(new Config
        {
            Audience = "bubbles-bot-gh-aud",
            IdRequestTokenEnvName = IdRequestTokenEnvName,
            IdRequestUrlEnvName = IdRequestUrlEnvName,
            SupabasePublishableKey = "",
            SupabaseRequestTokenUrl = SupabaseUrl
        });
    }

    internal void SetDefaultEnvVars()
    {
        Environment.SetEnvironmentVariable(IdRequestTokenEnvName, IdRequestToken);
        Environment.SetEnvironmentVariable(IdRequestUrlEnvName, IdRequestTokenUrl);
    }

    internal void ResetEnvVars()
    {
        Environment.SetEnvironmentVariable(IdRequestTokenEnvName, null);
        Environment.SetEnvironmentVariable(IdRequestUrlEnvName, null);
    }

    internal void SetupIdTokenResponse<TMock>(Mock<TMock> mock, HttpResponseMessage result) where TMock : class
    {
        mock.Protected()
            .Setup<HttpResponseMessage>(
                "Send",
                _idTokenResponseExpr,
                ItExpr.IsAny<CancellationToken>())
            .Returns(result);
    }

    internal void VerifyIdTokenResponseCalled<TMock>(Mock<TMock> mock) where TMock : class
    {
        mock.Protected()
            .Verify(
                "Send",
                Times.Once(),
                _idTokenResponseExpr,
                ItExpr.IsAny<CancellationToken>());
    }

    internal void SetupInstallationTokenResponse<TMock>(Mock<TMock> mock, HttpResponseMessage result) where TMock : class
    {
        mock.Protected()
            .Setup<HttpResponseMessage>(
                "Send",
                _installationTokenResponseExpr,
                ItExpr.IsAny<CancellationToken>())
            .Returns(result);
    }

    internal void VerifyInstallationTokenResponseCalled<TMock>(Mock<TMock> mock) where TMock : class
    {
        mock.Protected()
            .Verify(
                "Send",
                Times.Once(),
                _installationTokenResponseExpr,
                ItExpr.IsAny<CancellationToken>());
    }

    internal IGitHubClientFactory GetSubject(IOptions<Config> config, HttpClient client)
        => new GitHubClientFactory(config, client);
}