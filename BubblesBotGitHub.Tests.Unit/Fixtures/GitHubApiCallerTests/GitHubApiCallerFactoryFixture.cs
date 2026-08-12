using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using JetBrains.Annotations;
using Moq;

namespace BubblesBotGitHub.Tests.Unit.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public sealed class GitHubApiCallerFactoryFixture
{
    public const string RequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN";
    public const string RequestTokenEnvValue = "mock-value";
    public const string RequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL";
    public const string GitHubUserContentHost = "actions.githubusercontent.com";
    public const string SupabaseHost = "supabase.co";
    public const string MockOidcValue = """{"value":"fake-oidc-123"}""";
    public const string MockInstallationTokenValue = """{"value":"fake-installation-token-123"}""";
    public const string RequestUrlEnvValue = "https://pipelines.actions.githubusercontent.com/id-token?api-version=2.0";

    public readonly Mock<HttpMessageHandler> MockHttpHandler = new(MockBehavior.Strict);
    internal IGitHubApiCallerFactory GetFactory(HttpClient client) => new GitHubApiCallerFactory(client);
}