using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using BubblesBotGitHub.Tests.Unit.Fixtures;
using Octokit;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Models;
using Octokit.Webhooks.Models.PullRequestEvent;
using AuthorAssociation = Octokit.Webhooks.Models.AuthorAssociation;
using PullRequest = Octokit.Webhooks.Models.PullRequestEvent.PullRequest;
using Repository = Octokit.Webhooks.Models.Repository;
using User = Octokit.Webhooks.Models.User;

[assembly: AssemblyFixture(typeof(AssemblyFixture))]
namespace BubblesBotGitHub.Tests.Unit.Fixtures;

public class AssemblyFixture
{
    internal static IGitHubApiCaller CreateGitHubApiCaller(IGitHubClient client) => new GitHubApiCaller(client);

    private static readonly string RootWorkingDir = "./tmp";
    public static readonly string BaseSha = "a3f4edfee60026fc44989822ac8789e376f374a2";
    public static readonly string HeadSha = "1f85b89057373f54de739944889d1abec8c048b0";
    public static readonly string GitTestsDir = $"{RootWorkingDir}/GitTest";
    
    // ActionOptions related settings
    public static readonly string PostComment = "always";
    public static readonly bool AutoMerge = true;
    public static readonly string CustomCommand = "/fastforward";
    
    public static string RepoUrl => "https://github.com/LuneiSolei/Fast-Forward-Blossom-Bot-Tests.git";

    internal static PullRequestOpenedEvent CreatePullRequestOpenedEvent()
    {
        User mockUser = new User
        {
            Login = "luneisolei",
            AvatarUrl = "https://avatars.githubusercontent.com/u/68037318?v=4",
            Url = "https://github.com/LuneiSolei",
            Type = "user"
        };

        User mockOwner = new User
        {
            Login = "bubbles-bot-gh",
            AvatarUrl = "https://avatars.githubusercontent.com/u/101435117?v=4",
            Url = "https://github.com/bubbles-bot-gh",
            Type = "organization"
        };
        
        return new PullRequestOpenedEvent
        {
            PullRequest = new PullRequest
            {
                Url = "https://github.com/bubbles-bot-gh/fast-forward",
                NodeId = "1",
                HtmlUrl = "https://github.com/bubbles-bot-gh/fast-forward",
                DiffUrl = "https://github.com/bubbles-bot-gh/fast-forward.diff",
                PatchUrl = "https://github.com/bubbles-bot-gh/fast-forward.patch",
                IssueUrl = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/issues/1347",
                State = PullRequestState.Open,
                Title = "Mock Pull Request",
                User = mockUser,
                Assignees = [],
                RequestedReviewers = [],
                RequestedTeams = [],
                Labels = [],
                CommitsUrl = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/pulls/1347/commits",
                ReviewCommentsUrl = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/pulls/1347/comments",
                ReviewCommentUrl = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/comments/1",
                CommentsUrl = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/issues/1347/comments",
                StatusesUrl = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/statuses/abc123",
                Head = new PullRequestHead
                {
                    Label = "luneisolei:feature-branch",
                    Ref = "feature-branch",
                    Sha = "def456",
                    User = mockUser
                },
                Base = new PullRequestBase
                {
                    Label = "luneisolei:main",
                    Ref = "main",
                    Sha = "abc123",
                    User = mockUser,
                    Repo = new Repository
                    {
                        NodeId = "123",
                        Name = "fast-forward",
                        FullName = "bubbles-bot-gh/fast-forward",
                        Owner = mockOwner,
                        HtmlUrl = "https://github.com/bubbles-bot-gh/fast-forward",
                        Url = "https://github.com/bubbles-bot-gh/fast-forward"
                    }
                },
                Links = new PullRequestLinks
                {
                    Self = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/pulls/1347" },
                    Html = new Link{ Href = "https://github.com/bubbles-bot-gh/fast-forward/pull/1347" },
                    Issue = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/issues/1347" },
                    Comments = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/issues/1347/comments" },
                    ReviewComments = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/pulls/1347/comments" },
                    ReviewComment = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/pull/comments/1" },
                    Commits = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/pulls/1347/commits" },
                    Statuses = new Link{ Href = "https://api.github.com/repos/bubbles-bot-gh/fast-forward/statuses/abc123" },
                },
                AuthorAssociation = AuthorAssociation.Collaborator,
                MergeableState = "clean"
            }
        };
    }
}