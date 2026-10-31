using System.Linq.Expressions;
using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.FastForward.Infrastructure.Services;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

[UsedImplicitly]
public sealed class GitHubClientFactoryTestFixture
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
    private readonly PullRequestOpenedEvent _pullRequestOpenedEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private readonly ActionEventType _eventType = ActionEventType.PullRequestOpened;

    public GitHubClientFactoryTestFixture()
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

    internal IGitHubClient BuildGitHubClient(IOptions<Config> config, HttpClient client)
    {
        IHost host = AssemblyFixture.CreateApplicationBuilderWithMocks(
            _pullRequestOpenedEvent,
            _eventType,
            new MockServices
            {
                MockGitHubClient = null,
                MockGitHubClientFactory = null,
                MockHttpClient = null
            },
            services =>
            {
                services.RemoveAll<IGitHubClientFactory>();
                services.AddScoped<IGitHubClientFactory>(_ => new GitHubClientFactory(config, client));
            }
            ).Build();

        using IServiceScope scope = host.Services.CreateAsyncScope();
        return scope.ServiceProvider.GetRequiredService<IGitHubClient>();
    }
}