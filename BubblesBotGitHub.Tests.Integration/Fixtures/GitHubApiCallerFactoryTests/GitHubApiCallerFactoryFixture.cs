using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Integration.Fixtures.GitHubApiCallerFactoryTests;

[UsedImplicitly]
public sealed class GitHubApiCallerFactoryFixture
{
    // TODO: The request url env var is one actually used by GitHub!
    public readonly string RequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN";
    public readonly string RequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL";
    public readonly string RequestToken = "{ \"value\": \"fake-oidc-request-token\" }";
    public readonly string RequestUrl = "https://aathdejntmbwopbxmrzv.supabase.co/functions/v1/gh-app-auth";
    public readonly string InstallationAccessTokenMock = "abcdefghijklmnopqrstuvwxyz";
    public readonly string InstallationIdMock = "12345";
}