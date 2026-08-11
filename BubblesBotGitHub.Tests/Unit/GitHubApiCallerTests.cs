using System.Linq.Expressions;
using System.Net;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.Tests.Fixtures;
using BubblesBotGitHub.Tests.Fixtures.GitHubApiCallerTests;
using JetBrains.Annotations;
using Moq;
using Moq.Protected;
using Octokit;

namespace BubblesBotGitHub.Tests.Unit;

[UsedImplicitly]
public sealed class GitHubApiCallerTests
{
    public class GitHubApiCallerFactory(GitHubApiCallerFactoryFixture classFixture) 
        : IClassFixture<GitHubApiCallerFactoryFixture>
    {
        [Fact]
        public void SucceedsCreation()
        {
            Environment.SetEnvironmentVariable(
                GitHubApiCallerFactoryFixture.RequestTokenEnvName, 
                GitHubApiCallerFactoryFixture.RequestTokenEnvValue);
            Environment.SetEnvironmentVariable(
                GitHubApiCallerFactoryFixture.RequestUrlEnvName, 
                GitHubApiCallerFactoryFixture.RequestUrlEnvValue);
            
            // Mock setup
            classFixture.MockHttpHandler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get 
                        && req.RequestUri!.Host.Contains(GitHubApiCallerFactoryFixture.GitHubUserContentHost)
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(GitHubApiCallerFactoryFixture.MockOidcValue)
                    });

            classFixture.MockHttpHandler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                        && req.RequestUri!.Host.Contains(GitHubApiCallerFactoryFixture.SupabaseHost)
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(GitHubApiCallerFactoryFixture.MockInstallationTokenValue)
                    });
            
            // Get subject with mocked object
            IGitHubApiCallerFactory factory = classFixture.GetFactory(new HttpClient(classFixture.MockHttpHandler.Object));
            IGitHubApiCaller apiCaller = factory.Create();
            
            // Verify results
            Assert.NotNull(apiCaller);
            classFixture.MockHttpHandler.Protected().Verify(
                "Send",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.RequestUri!.Host.Contains(GitHubApiCallerFactoryFixture.GitHubUserContentHost)),
                ItExpr.IsAny<CancellationToken>());
            
            classFixture.MockHttpHandler.Protected().Verify(
                "Send",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.RequestUri!.Host.Contains(GitHubApiCallerFactoryFixture.SupabaseHost)),
                ItExpr.IsAny<CancellationToken>());
        }
    }
    
    public sealed class GetPullRequest(AssemblyFixture assemblyFixture, GetPullRequestFixture classFixture) : IClassFixture<GetPullRequestFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync() { return ValueTask.CompletedTask; }

        public ValueTask DisposeAsync()
        {
            try
            {
                classFixture.MockOctokitClient.Reset();
                return ValueTask.CompletedTask;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }
        }
        
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = GetPullRequestFixture.Owner;
            string name = GetPullRequestFixture.Name;
            PullRequest expected = classFixture.SuccessExpected;
            int prNumber = classFixture.SuccessExpected.Number;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<PullRequest>>> mockExpr = client =>
                client.PullRequest.Get(owner, name, prNumber);
            
            classFixture.MockOctokitClient.Setup(mockExpr).ReturnsAsync(expected);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            PullRequest result = await subject.GetPullRequest(owner, name, (uint)prNumber);
            
            // Verify results
            Assert.Equal(expected.Number, result.Number);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = GetPullRequestFixture.Owner;
            string name = GetPullRequestFixture.Name;
            uint prNumber = GetPullRequestFixture.FailedPrNumber;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<PullRequest>>> mockExpr = client =>
                client.PullRequest.Get(owner, name, (int)prNumber);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => 
                subject.GetPullRequest(owner, name, prNumber));
            
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class GetBaseHeadComparison(AssemblyFixture assemblyFixture, GetBaseHeadComparisonFixture classFixture) :
        IClassFixture<GetBaseHeadComparisonFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync() { return ValueTask.CompletedTask; }

        public ValueTask DisposeAsync()
        {
            try
            {
                classFixture.MockOctokitClient.Reset();
                return ValueTask.CompletedTask;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }
        }
        
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = GetBaseHeadComparisonFixture.Owner;
            string name = GetBaseHeadComparisonFixture.Name;
            string baseSha = GetBaseHeadComparisonFixture.BaseSha;
            string headLabel = GetBaseHeadComparisonFixture.HeadLabel;
            CompareResult expected = classFixture.SuccessExpected;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<CompareResult>>> mockExpr = client =>
                client.Repository.Commit.Compare(owner, name, baseSha, headLabel);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ReturnsAsync(expected);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            CompareResult result = await subject.GetBaseHeadComparison(owner, name, baseSha, headLabel);

            // Verify results
            Assert.Equal(expected.Status, result.Status);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = GetBaseHeadComparisonFixture.Owner;
            string name = GetBaseHeadComparisonFixture.Name;
            string baseSha = GetBaseHeadComparisonFixture.BaseSha;
            string headLabel = GetBaseHeadComparisonFixture.HeadLabel;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<CompareResult>>> mockExpr = client => 
                client.Repository.Commit.Compare(owner, name, baseSha, headLabel);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() =>
                subject.GetBaseHeadComparison(owner, name, baseSha, headLabel));

            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class IsCollaborator(AssemblyFixture assemblyFixture, IsCollaboratorFixture classFixture) : IClassFixture<IsCollaboratorFixture>,  IAsyncLifetime
    {
        public ValueTask InitializeAsync() { return ValueTask.CompletedTask; }

        public ValueTask DisposeAsync()
        {
            try
            {
                classFixture.MockOctokitClient.Reset();
                return ValueTask.CompletedTask;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }
        }
        
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = IsCollaboratorFixture.Owner;
            string name = IsCollaboratorFixture.Name;
            string user = IsCollaboratorFixture.User;
            bool expected = IsCollaboratorFixture.SuccessExpected;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<bool>>> mockExpr = client =>
                client.Repository.Collaborator.IsCollaborator(owner, name, user);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ReturnsAsync(expected);

            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            bool result = await subject.IsCollaborator(owner, name, user);

            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner  = IsCollaboratorFixture.Owner;
            string name = IsCollaboratorFixture.Name;
            string user = IsCollaboratorFixture.User;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<bool>>> mockExpr = client =>
                client.Repository.Collaborator.IsCollaborator(owner, name, user);
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => subject.IsCollaborator(owner, name, user));
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class PostComment(AssemblyFixture assemblyFixture, PostCommentFixture classFixture) : IClassFixture<PostCommentFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync() { return ValueTask.CompletedTask; }

        public ValueTask DisposeAsync()
        {
            try
            {
                classFixture.MockOctokitClient.Reset();
                return ValueTask.CompletedTask;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }
        }

        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = PostCommentFixture.Owner;
            string name = PostCommentFixture.Name;
            uint issueNumber = PostCommentFixture.IssueNumber;
            IssueComment expected = classFixture.SuccessExpected;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<IssueComment>>> mockExpr = client =>
                client.Issue.Comment.Create(owner, name, issueNumber, expected.Body);

            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ReturnsAsync(expected);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            IssueComment result = await subject.PostComment(owner, name, issueNumber, expected.Body);

            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = PostCommentFixture.Owner;
            string name = PostCommentFixture.Name;
            uint issueNumber = PostCommentFixture.IssueNumber;
            IssueComment expected = classFixture.FailureExpected;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<IssueComment>>> mockExpr = client =>
                client.Issue.Comment.Create(owner, name, issueNumber, expected.Body);

            classFixture.MockOctokitClient.Setup(mockExpr).ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => 
                subject.PostComment(owner, name, issueNumber, expected.Body));
            
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class GetCommit(AssemblyFixture assemblyFixture, GetCommitFixture classFixture) 
        : IClassFixture<GetCommitFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync() { return ValueTask.CompletedTask; }

        public ValueTask DisposeAsync()
        {
            try
            {
                classFixture.MockOctokitClient.Reset();
                return ValueTask.CompletedTask;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }
        }

        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = GetCommitFixture.Owner;
            string name = GetCommitFixture.Name;
            GitHubCommit expected = classFixture.SuccessExpected;
            string sha = classFixture.SuccessExpected.Sha;

            // Mock setup
            Expression<Func<IGitHubClient, Task<GitHubCommit>>> mockExpr = client =>
                client.Repository.Commit.Get(owner, name, sha);

            classFixture.MockOctokitClient.Setup(mockExpr).ReturnsAsync(expected);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            GitHubCommit result = await subject.GetCommit(owner, name, sha);
            
            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = GetCommitFixture.Owner;
            string name = GetCommitFixture.Name;
            string sha = classFixture.SuccessExpected.Sha;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<GitHubCommit>>> mockExpr = client =>
                client.Repository.Commit.Get(owner, name, sha);
            classFixture.MockOctokitClient.Setup(mockExpr).ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => subject.GetCommit(owner, name, sha));
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class FastForward(AssemblyFixture assemblyFixture, FastForwardFixture classFixture)
        : IClassFixture<FastForwardFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            try
            {
                classFixture.MockOctokitClient.Reset();
                return ValueTask.CompletedTask;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }
        }

        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = FastForwardFixture.Owner;
            string name = FastForwardFixture.Name;
            string headSha = FastForwardFixture.HeadSha;
            string baseLabel = FastForwardFixture.BaseLabel;
            Reference expected = classFixture.ExpectedSuccess;
            
            // Mock setup
            Expression<Func<IGitHubClient, Task<Reference>>> mockExpr = client =>
                client.Git.Reference.Update(
                    owner, 
                    name, 
                    reference: baseLabel, 
                    referenceUpdate: It.Is<ReferenceUpdate>(
                        refUpdate => refUpdate.Sha == headSha && refUpdate.Force == false)
                    );
            classFixture.MockOctokitClient.Setup(mockExpr).ReturnsAsync(expected);
            
            // Get subject with mocked object
            IGitHubApiCaller subject = AssemblyFixture.CreateGitHubApiCaller(classFixture.MockOctokitClient.Object);
            Reference result = await subject.FastForward(owner, name, baseLabel, headSha);
            
            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }
}