using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using BubblesBotGitHub.Tests.Integration.Fixtures.GitHubApiCallerFactoryTests;
using JetBrains.Annotations;

namespace BubblesBotGitHub.Tests.Integration.Tests;

[UsedImplicitly]
[Collection("GitHubApiCallerFactoryIntegrationTests")]
public sealed class GitHubApiCallerFactoryTests(GetOidcTokenFixture classFixture) : IClassFixture<GetOidcTokenFixture>, IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable(GetOidcTokenFixture.RequestTokenEnvName, GetOidcTokenFixture.RequestToken);
        Environment.SetEnvironmentVariable(GetOidcTokenFixture.RequestUrlEnvName, GetOidcTokenFixture.RequestUrl);
            
        return ValueTask.CompletedTask;
    }
        
    public ValueTask DisposeAsync()
    {
        Environment.SetEnvironmentVariable(GetOidcTokenFixture.RequestTokenEnvName, null);
        Environment.SetEnvironmentVariable(GetOidcTokenFixture.RequestUrlEnvName, null);
            
        return ValueTask.CompletedTask;
    }

    [Fact]
    public void SuccessfullyExchangesOidcAndInstallationTokens()
    {
        MockGitHubAuthHandler handler = new();
        GitHubApiCallerFactory factory = new(new HttpClient(handler));
        IGitHubApiCaller caller = factory.Create();

        Assert.NotNull(caller);
    }

    [Fact]
    public void ThrowsExceptionWhenIdTokenIsNotSet()
    {
        Environment.SetEnvironmentVariable(GetOidcTokenFixture.RequestTokenEnvName, null);
        MockGitHubAuthHandler handler = new();
        GitHubApiCallerFactory factory = new(new HttpClient(handler));
        
        Assert.Throws<InvalidOperationException>(factory.Create);
    }

    [Fact]
    public void ThrowsWhenUrlIsNotSet()
    {
        Environment.SetEnvironmentVariable(GetOidcTokenFixture.RequestUrlEnvName, null);
        MockGitHubAuthHandler handler = new();
        GitHubApiCallerFactory factory = new(new HttpClient(handler));
        
        Assert.Throws<InvalidOperationException>(factory.Create);
    }
}