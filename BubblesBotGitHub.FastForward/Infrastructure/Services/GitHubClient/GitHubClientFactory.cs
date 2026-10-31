using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BubblesBotGitHub.FastForward.Core.Entities;
using Microsoft.Extensions.Options;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services;

internal class GitHubClientFactory(IOptions<Config> opts, HttpClient httpClient) : IGitHubClientFactory
{
    private readonly string _requestTokenUrl = opts.Value.SupabaseRequestTokenUrl;
    
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
        string reqToken = Environment.GetEnvironmentVariable(opts.Value.IdRequestTokenEnvName) 
            ?? throw new InvalidOperationException("ID token not set");
        
        string reqUrl = Environment.GetEnvironmentVariable(opts.Value.IdRequestUrlEnvName) 
            ?? throw new InvalidOperationException("URL not set");
        
        HttpRequestMessage msg = new(HttpMethod.Get, $"{reqUrl}&audience={opts.Value.Audience}");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", reqToken);

        HttpResponseMessage res = httpClient.Send(msg);
        res.EnsureSuccessStatusCode();

        string json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using JsonDocument doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("value").GetString()
            ?? throw new InvalidOperationException("OIDC response 'value' field is null");
    }

    private string GetInstallationToken(string oidcToken)
    {
        HttpRequestMessage msg = new(HttpMethod.Post, _requestTokenUrl)
        {
            Content = JsonContent.Create(new { token = oidcToken })
        };
        
        msg.Headers.Add("apikey", opts.Value.SupabasePublishableKey);
        HttpResponseMessage res = httpClient.Send(msg);
        res.EnsureSuccessStatusCode();
        
        string json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using JsonDocument doc = JsonDocument.Parse(json);
        
        if (doc.RootElement.ValueKind != JsonValueKind.Object
            || !doc.RootElement.TryGetProperty("token", out JsonElement tokenElement)
            || tokenElement.GetString() is not { Length: > 0 } installationToken)
        {
            string shape = doc.RootElement.ValueKind == JsonValueKind.Object
                ? "keys: " + string.Join(", ", doc.RootElement.EnumerateObject().Select(p => p.Name))
                : $"root was {doc.RootElement.ValueKind}";
            throw new InvalidOperationException(
                $"Installation token response had no usable 'token' field ({shape}).");
        }

        return installationToken;
    }
}