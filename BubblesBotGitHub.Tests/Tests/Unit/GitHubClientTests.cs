using System.Linq.Expressions;
using BubblesBotGitHub.Tests.Fixtures.GitHubClientTests;
using JetBrains.Annotations;
using Moq;
using Octokit;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.Tests.Tests.Unit;

[UsedImplicitly]
public sealed class GitHubClientTests
{
    public sealed class GetPullRequest(GetPullRequestFixture classFixture) : IClassFixture<GetPullRequestFixture>
    {
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            PullRequest expected = classFixture.SuccessExpected;
            int prNumber = classFixture.SuccessExpected.Number;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<PullRequest>>> mockExpr = client =>
                client.PullRequest.Get(owner, name, prNumber);
            
            classFixture.MockOctokitClient.Setup(mockExpr).ReturnsAsync(expected);
            
            // Get subject with mocked object
            PullRequest result = await classFixture.Subject.GetPullRequestAsync(owner, name, prNumber);
            
            // Verify results
            Assert.Equal(expected.Number, result.Number);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            int prNumber = classFixture.FailedPrNumber;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<PullRequest>>> mockExpr = client =>
                client.PullRequest.Get(owner, name, prNumber);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ThrowsAsync(classFixture.NotFoundException);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => 
                classFixture.Subject.GetPullRequestAsync(owner, name, prNumber));
            
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class GetBaseHeadComparison(GetBaseHeadComparisonFixture classFixture) :
        IClassFixture<GetBaseHeadComparisonFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync() => ValueTask.CompletedTask;

        public async ValueTask DisposeAsync() => await classFixture.DisposeAsync();

        [Fact]
        public async Task SucceedsWhenValid()
        {
            CompareResult expected = classFixture.SuccessExpected;
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            string baseSha = classFixture.BaseSha;
            string headLabel = classFixture.HeadLabel;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<CompareResult>>> mockExpr = client =>
                client.Repository.Commit.Compare(owner, name, baseSha, headLabel);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ReturnsAsync(expected);
            
            // Get subject with mocked object
            CompareResult result = await classFixture.Subject.GetBaseHeadComparison(owner, name, baseSha, headLabel);

            // Verify results
            Assert.Equal(expected.Status, result.Status);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            string baseSha = classFixture.BaseSha;
            string headLabel = classFixture.HeadLabel;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<CompareResult>>> mockExpr = client => 
                client.Repository.Commit.Compare(owner, name, baseSha, headLabel);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ThrowsAsync(classFixture.NotFoundException);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() =>
                classFixture.Subject.GetBaseHeadComparison(owner, name, baseSha, headLabel));

            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class IsCollaborator(IsCollaboratorFixture classFixture) 
        : IClassFixture<IsCollaboratorFixture>, IAsyncLifetime
    {
        public async ValueTask InitializeAsync() => await classFixture.InitializeAsync();

        public async ValueTask DisposeAsync() => await classFixture.DisposeAsync();
        
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            string user = classFixture.User;
            bool expected = classFixture.SuccessExpected;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<bool>>> mockExpr = client =>
                client.Repository.Collaborator.IsCollaborator(owner, name, user);
            
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ReturnsAsync(expected);

            // Get subject with mocked object
            bool result = await classFixture.Subject.IsCollaborator(owner, name, user);

            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner  = classFixture.Owner;
            string name = classFixture.Name;
            string user = classFixture.User;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<bool>>> mockExpr = client =>
                client.Repository.Collaborator.IsCollaborator(owner, name, user);
            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubClient subject = classFixture.Subject;
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => subject.IsCollaborator(owner, name, user));
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class PostComment(PostCommentFixture classFixture) : IClassFixture<PostCommentFixture>
    {
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            uint issueNumber = classFixture.IssueNumber;
            IssueComment expected = classFixture.SuccessExpected;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<IssueComment>>> mockExpr = client =>
                client.Issue.Comment.Create(owner, name, issueNumber, expected.Body);

            classFixture.MockOctokitClient
                .Setup(mockExpr)
                .ReturnsAsync(expected);
            
            // Get subject with mocked object
            IssueComment result = await classFixture.Subject.PostComment(owner, name, issueNumber, expected.Body);

            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            uint issueNumber = classFixture.IssueNumber;
            IssueComment expected = classFixture.FailureExpected;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<IssueComment>>> mockExpr = client =>
                client.Issue.Comment.Create(owner, name, issueNumber, expected.Body);

            classFixture.MockOctokitClient.Setup(mockExpr).ThrowsAsync(classFixture.NotFoundException);
            
            // Get subject with mocked object
            IGitHubClient subject = classFixture.Subject;

            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => 
                subject.PostComment(owner, name, issueNumber, expected.Body));
            
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class GetCommit(GetCommitFixture classFixture) : IClassFixture<GetCommitFixture>, IAsyncLifetime
    {
        public ValueTask InitializeAsync() => ValueTask.CompletedTask;
        
        public async ValueTask DisposeAsync() => await classFixture.DisposeAsync();
        
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            GitHubCommit expected = classFixture.SuccessExpected;
            string sha = classFixture.SuccessExpected.Sha;

            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<GitHubCommit>>> mockExpr = client =>
                client.Repository.Commit.Get(owner, name, sha);

            classFixture.MockOctokitClient.Setup(mockExpr).ReturnsAsync(expected);
            
            // Get subject with mocked object
            GitHubCommit result = await classFixture.Subject.GetCommit(owner, name, sha);
            
            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }

        [Fact]
        public async Task ThrowsWhenFailed()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            string sha = classFixture.SuccessExpected.Sha;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<GitHubCommit>>> mockExpr = client =>
                client.Repository.Commit.Get(owner, name, sha);
            classFixture.MockOctokitClient.Setup(mockExpr).ThrowsAsync(classFixture.NotFoundException);
            
            // Verify results
            await Assert.ThrowsAsync<NotFoundException>(() => 
                classFixture.Subject.GetCommit(owner, name, sha));
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }

    public sealed class FastForward(FastForwardFixture classFixture)
        : IClassFixture<FastForwardFixture>
    {
        [Fact]
        public async Task SucceedsWhenValid()
        {
            string owner = classFixture.Owner;
            string name = classFixture.Name;
            string headSha = classFixture.HeadSha;
            string baseLabel = classFixture.BaseLabel;
            Reference expected = classFixture.ExpectedSuccess;
            
            // Mock setup
            Expression<Func<Octokit.IGitHubClient, Task<Reference>>> mockExpr = client =>
                client.Git.Reference.Update(
                    owner, 
                    name, 
                    reference: baseLabel, 
                    referenceUpdate: It.Is<ReferenceUpdate>(
                        refUpdate => refUpdate.Sha == headSha && refUpdate.Force == false)
                    );
            classFixture.MockOctokitClient.Setup(mockExpr).ReturnsAsync(expected);
            
            // Get subject with mocked object
            Reference result = await classFixture.Subject.FastForward(owner, name, baseLabel, headSha);
            
            // Verify results
            Assert.Equal(expected, result);
            classFixture.MockOctokitClient.Verify(mockExpr, Times.Once);
        }
    }
}