using System.Net;
using System.Text.Json;

namespace BubblesBotGitHub.Tests.Integration.Fixtures.GitHubApiCallerFactoryTests;

public sealed class MockGitHubAuthHandler : HttpMessageHandler
{
    public const string InstallationToken = "fake-installation-token";
    private const string SupabaseHost = "aathdejntmbwopbxmrzv.supabase.co";
    public HttpStatusCode OidcResponseStatus { get; set; } = HttpStatusCode.OK;
    public HttpStatusCode InstallationResponseStatus { get; set; } = HttpStatusCode.OK;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Send(request, cancellationToken));
    }

    protected override HttpResponseMessage Send(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Uri requestUri = request.RequestUri != null 
            ? request.RequestUri 
            : throw new NullReferenceException();
        
        HttpResponseMessage response = requestUri.Host == SupabaseHost
            ? BuildResponse(InstallationResponseStatus, InstallationToken)
            : BuildResponse(OidcResponseStatus, GetOidcTokenFixture.RequestToken);

        return response;
    }

    private static HttpResponseMessage BuildResponse(HttpStatusCode status, string value)
    {
        string body = status == HttpStatusCode.OK
            ? JsonSerializer.Serialize(new { value })
            : """{"error":"simulated failure"}""";

        return new HttpResponseMessage(status)
        {
            Content = new StringContent(body)
        };
    }
}