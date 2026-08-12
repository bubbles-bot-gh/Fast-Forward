using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Integration.Fixtures.GitHubApiCallerFactoryTests;

[UsedImplicitly]
public sealed class GetOidcTokenFixture
{
    public const string RequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN";
    public const string RequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL";
    public const string RequestToken = "fake-oidc-request-token";
    public const string RequestUrl = "https://fake-actions-oidc.example/token?";
}