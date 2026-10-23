// using System.Net;
// using BubblesBotGitHub.FastForward.Core.Entities;
// using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
// using BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;
// using FluentAssertions;
// using Microsoft.Extensions.Options;
// using Moq;
// using Moq.Protected;
//
// namespace BubblesBotGitHub.Tests.Tests.Unit;
//
// public class GitHubClientFactoryTestsOld(GitHubClientFactoryFixture classFixture) 
//     : IClassFixture<GitHubClientFactoryFixture>
// {
//     [Fact]
//     public void SucceedsCreatingInstance()
//     {
//         Environment.SetEnvironmentVariable(classFixture.RequestTokenEnvName, classFixture.RequestTokenEnvValue);
//         Environment.SetEnvironmentVariable(classFixture.RequestUrlEnvName, classFixture.RequestUrlEnvValue);
//         
//         // Mock setup
//         classFixture
//             .MockHttpHandler
//             .Protected()
//             .Setup<HttpResponseMessage>(
//                 "Send",
//                 ItExpr.Is<HttpRequestMessage>(req =>
//                     req.Method == HttpMethod.Get 
//                     && req.RequestUri!.Host.Contains(classFixture.GitHubUserContentHost)
//                 ),
//                 ItExpr.IsAny<CancellationToken>())
//             .Returns(
//                 new HttpResponseMessage
//                 {
//                     StatusCode = HttpStatusCode.OK,
//                     Content = new StringContent(classFixture.IdTokenResponseMock)
//                 });
//
//         classFixture
//             .MockHttpHandler
//             .Protected()
//             .Setup<HttpResponseMessage>(
//                 "Send",
//                 ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
//                     && req.RequestUri!.Host.Contains(classFixture.SupabaseHost)
//                 ),
//                 ItExpr.IsAny<CancellationToken>())
//             .Returns(
//                 new HttpResponseMessage
//                 {
//                     StatusCode = HttpStatusCode.OK,
//                     Content = new StringContent(classFixture.InstallationTokenResponseMock)
//                 });
//         
//         // Get subject with mocked object
//         IGitHubClientFactory factory = classFixture.GetFactory(new HttpClient(classFixture.MockHttpHandler.Object));
//         IGitHubClient client = factory.Create();
//         
//         // Verify results
//         Assert.NotNull(client);
//         classFixture.MockHttpHandler.Protected().Verify(
//             "Send",
//             Times.Once(),
//             ItExpr.Is<HttpRequestMessage>(req => 
//                 req.RequestUri!.Host.Contains(classFixture.GitHubUserContentHost)),
//             ItExpr.IsAny<CancellationToken>());
//         
//         classFixture.MockHttpHandler.Protected().Verify(
//             "Send",
//             Times.Once(),
//             ItExpr.Is<HttpRequestMessage>(req => 
//                 req.RequestUri!.Host.Contains(classFixture.SupabaseHost)),
//             ItExpr.IsAny<CancellationToken>());
//
//         classFixture.MockHttpHandler.Reset();
//     }
//
//     [Collection("Create")]
//     public class Create(GitHubClientFactoryFixture classFixture) : IClassFixture<GitHubClientFactoryFixture>
//     {
//         [Fact]
//         public void IdRequestTokenNotSet_Throws()
//         {
//             IOptions<Config> options = Options.Create(
//                 new Config
//                 {
//                     Audience = "bubbles-bot-gh-aud",
//                     IdRequestTokenEnvName = "some-invalid-env-name",
//                     IdRequestUrlEnvName = "",
//                     SupabasePublishableKey = "",
//                     SupabaseRequestTokenUrl = ""
//                 });
//
//             HttpClient httpClient = new Mock<HttpClient>().Object;
//
//             IGitHubClientFactory subject = new GitHubClientFactory(options, httpClient);
//             Action action = () => subject.Create();
//
//             action.Should().Throw<InvalidOperationException>("ID token not set");
//         }
//
//         [Fact]
//         public void IdRequestUrlNotSet_Throws()
//         {
//             IOptions<Config> options = Options.Create(
//                 new Config
//                 {
//                     Audience = "bubbles-bot-gh-aud",
//                     IdRequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN",
//                     IdRequestUrlEnvName = "some-invalid-url-env-name",
//                     SupabasePublishableKey = "",
//                     SupabaseRequestTokenUrl = ""
//                 });
//
//             Environment.SetEnvironmentVariable(options.Value.IdRequestTokenEnvName, "some-token");
//
//             HttpClient httpClient = new Mock<HttpClient>().Object;
//             IGitHubClientFactory subject = new GitHubClientFactory(options, httpClient);
//             Action action = () => subject.Create();
//
//             action.Should().Throw<InvalidOperationException>("URL not set");
//         }
//
//         [Fact]
//         public void OidcResponseHasNoValueField_Throws()
//         {
//             IOptions<Config> options = Options.Create(
//                 new Config
//                 {
//                     Audience = "bubbles-bot-gh-aud",
//                     IdRequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN",
//                     IdRequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL",
//                     SupabasePublishableKey = "",
//                     SupabaseRequestTokenUrl = ""
//                 });
//             Environment.SetEnvironmentVariable(options.Value.IdRequestTokenEnvName, "some-token");
//             Environment.SetEnvironmentVariable(options.Value.IdRequestUrlEnvName, "http://localhost/");
//             
//             classFixture
//                 .MockHttpHandler
//                 .Protected()
//                 .Setup<HttpResponseMessage>(
//                     "Send",
//                     ItExpr.IsAny<HttpRequestMessage>(),
//                     ItExpr.IsAny<CancellationToken>())
//                 .Returns(() => new HttpResponseMessage(HttpStatusCode.OK)
//                 {
//                     Content = new StringContent("{ \"value\": null }")
//                 });
//             
//             IGitHubClientFactory subject = classFixture.GetFactory(options, new HttpClient(classFixture.MockHttpHandler.Object));
//             Action action = () => subject.Create();
//
//             action.Should().Throw<InvalidOperationException>("OIDC response has no 'value' field");
//             classFixture.MockHttpHandler.Reset();
//         }
//
//         [Fact]
//         public void InstallationTokenResponseHasNoTokenField_Throws()
//         {
//             IOptions<Config> options = Options.Create(
//                 new Config
//                 {
//                     Audience = "bubbles-bot-gh-aud",
//                     IdRequestTokenEnvName = "ACTIONS_ID_TOKEN_REQUEST_TOKEN",
//                     IdRequestUrlEnvName = "ACTIONS_ID_TOKEN_REQUEST_URL",
//                     SupabasePublishableKey = "",
//                     SupabaseRequestTokenUrl = ""
//                 });
//             Environment.SetEnvironmentVariable(options.Value.IdRequestTokenEnvName, "some-token");
//             Environment.SetEnvironmentVariable(options.Value.IdRequestUrlEnvName, "http://localhost/");
//             
//             classFixture
//                 .MockHttpHandler
//                 .Protected()
//                 .SetupSequence<HttpResponseMessage>(
//                     "Send",
//                     ItExpr.IsAny<HttpRequestMessage>(),
//                     ItExpr.IsAny<CancellationToken>())
//                 .Returns(() => new HttpResponseMessage(HttpStatusCode.OK)
//                 {
//                     Content = new StringContent("{ \"value\": \"some-token\" }")
//                 })
//                 .Returns(() => new HttpResponseMessage(HttpStatusCode.OK)
//                 {
//                     Content = new StringContent("{ \"token\": null }")
//                 });
//
//             classFixture
//                 .MockHttpHandler
//                 .Protected()
//                 .Setup<HttpResponseMessage>(
//                     "Send",
//                     ItExpr.IsAny<HttpRequestMessage>(),
//                     ItExpr.IsAny<CancellationToken>());
//                 
//             IGitHubClientFactory subject = classFixture.GetFactory(options, new HttpClient(classFixture.MockHttpHandler.Object));
//             Action action = () => subject.Create();
//         
//             action.Should().Throw<InvalidOperationException>("Installation token response has no 'token' field");
//             classFixture.MockHttpHandler.Reset();
//         }
//     }
// }