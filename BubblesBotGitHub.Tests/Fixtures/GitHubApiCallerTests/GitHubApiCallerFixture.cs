using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;

[UsedImplicitly]
public class GitHubApiCallerFixture
{
    public readonly string RequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN";
    public readonly string RequestTokenEnvValue = "mock-value";
    public readonly string RequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL";
    public readonly string GitHubUserContentHost = "actions.githubusercontent.com";
    public readonly string SupabaseHost = "supabase.co";
    public readonly string MockOidcValue = """{"value":"fake-oidc-123"}""";
    public readonly string MockInstallationTokenValue = """{"value":"fake-installation-token-123"}""";

    public readonly string RequestUrlEnvValue =
        "https://pipelines.actions.githubusercontent.com/id-token?api-version=2.0";
}