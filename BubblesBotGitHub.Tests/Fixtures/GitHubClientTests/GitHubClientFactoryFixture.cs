using BubblesBotGitHub.FastForward.Core.Entities;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Moq;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;

[UsedImplicitly]
public sealed class GitHubClientFactoryFixture
{
    public readonly string RequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN";
    public readonly string RequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL";
    public readonly string RequestTokenEnvValue = "mock-value";
    public readonly string GitHubUserContentHost = "actions.githubusercontent.com";
    public readonly string SupabaseHost = "supabase.co";
    public readonly string MockOidcValue = """{"value":"fake-oidc-123"}""";
    public readonly string MockInstallationTokenValue = """{"value":"fake-installation-token-123"}""";
    
    // TODO: Is this the right resource?
    public readonly string RequestUrlEnvValue = "https://pipelines.actions.githubusercontent.com/id-token?api-version=2.0";

    public readonly Mock<HttpMessageHandler> MockHttpHandler = new(MockBehavior.Strict);
    private readonly IOptions<Config> _optionsMock = Options.Create(new Config
    {
        IdRequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN",
        SupabaseRequestTokenUrl = "https://aathdejntmbwopbxmrzv.supabase.co/functions/v1/gh-app-auth",
        IdRequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL",
    });

    internal IGitHubClientFactory GetFactory(HttpClient client) => new GitHubClientFactory(_optionsMock, client);
}