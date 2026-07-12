using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;

internal class GitHubApiCallerFactory(HttpClient httpClient) : IGitHubApiCallerFactory
{
    public IGitHubApiCaller Create()
    {
        string oidcToken = GetOidcToken();
        string installationToken = GetInstallationToken(oidcToken);
        
        IGitHubClient octokitClient = new GitHubClient(new ProductHeaderValue("BubblesBotGitHub.FastForward"))
        {
            Credentials = new Credentials(installationToken, AuthenticationType.Bearer)
        };
        
        return new GitHubApiCaller(octokitClient);
    }

    private string GetOidcToken()
    {
        // Extract OIDC related vars
        // "permissions.id-token" must be set to "write" in the user workflow in order for this to function
        string reqToken = Environment.GetEnvironmentVariable("ACTIONS_ID_TOKEN_REQUEST_TOKEN") 
            ?? throw new InvalidOperationException("ID token not set");
        
        string reqUrl = Environment.GetEnvironmentVariable("ACTIONS_ID_TOKEN_REQUEST_URL") 
            ?? throw new InvalidOperationException("URL not set");
        
        HttpRequestMessage msg = new(HttpMethod.Get, $"{reqUrl}&audience=bubbles-bot-gh-aud");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", reqToken);

        HttpResponseMessage res = httpClient.Send(msg);
        res.EnsureSuccessStatusCode();

        string json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using JsonDocument doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("value").GetString()
            ?? throw new InvalidOperationException("OIDC response has no 'value' field");
    }

    private string GetInstallationToken(string oidcToken)
    {
        HttpRequestMessage msg = new(
            HttpMethod.Post,
            "https://aathdejntmbwopbxmrzv.supabase.co/functions/v1/github-app-verification")
        {
            Content = JsonContent.Create(new { token = oidcToken })
        };

        HttpResponseMessage res = httpClient.Send(msg);
        res.EnsureSuccessStatusCode();
        
        string json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using JsonDocument doc = JsonDocument.Parse(json);
        
        return doc.RootElement.GetProperty("value").GetString()
            ?? throw new InvalidOperationException("Installation token response has no 'value' field");
    }
}