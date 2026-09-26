using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BubblesBotGitHub.FastForward.Core.Entities;
using Microsoft.Extensions.Options;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;

internal class GitHubClientFactory(IOptions<Config> opts, HttpClient httpClient) : IGitHubClientFactory
{
    private const string RequestTokenUrl = "https://aathdejntmbwopbxmrzv.supabase.co/functions/v1/gh-app-auth";
    public IGitHubClient Create()
    {
        string oidcToken = GetOidcToken();
        string installationToken = GetInstallationToken(oidcToken);
        
        Octokit.IGitHubClient octokitClient = new Octokit.GitHubClient(new ProductHeaderValue("BubblesBotGitHub.FastForward"))
        {
            Credentials = new Credentials(installationToken, AuthenticationType.Bearer)
        };
        
        return new GitHubClient(octokitClient);
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
        HttpRequestMessage msg = new(HttpMethod.Post, RequestTokenUrl)
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