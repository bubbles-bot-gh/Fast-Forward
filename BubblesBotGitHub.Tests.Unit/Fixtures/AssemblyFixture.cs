using System.Text.Json;
using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using BubblesBotGitHub.FastForward.Implements.GitHubApiCaller;
using BubblesBotGitHub.Tests.Unit.Fixtures;
using Moq;
using Octokit;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using AuthorAssociation = Octokit.Webhooks.Models.AuthorAssociation;
using IssueComment = Octokit.Webhooks.Models.IssueComment;
using PullRequest = Octokit.Webhooks.Models.PullRequestEvent.PullRequest;
using User = Octokit.Webhooks.Models.User;

[assembly: AssemblyFixture(typeof(AssemblyFixture))]
namespace BubblesBotGitHub.Tests.Unit.Fixtures;

public class AssemblyFixture
{
    internal static IGitHubApiCaller CreateGitHubApiCaller(IGitHubClient client) => new GitHubApiCaller(client);

    private static readonly string RootWorkingDir = "./tmp";
    private const string PrOpenedEventJson = "./Events/PullRequestOpenedEvent.json";
    public static readonly string BaseSha = "a3f4edfee60026fc44989822ac8789e376f374a2";
    public static readonly string HeadSha = "1f85b89057373f54de739944889d1abec8c048b0";
    public static readonly string GitTestsDir = $"{RootWorkingDir}/GitTest";
    
    private static readonly User MockUser = new()
    {
        Login = "luneisolei",
        AvatarUrl = "https://avatars.githubusercontent.com/u/68037318?v=4",
        Url = "https://github.com/LuneiSolei",
        Type = "user"
    };

    private static readonly User MockOwner = new()
    {
        Login = "bubbles-bot-gh",
        AvatarUrl = "https://avatars.githubusercontent.com/u/101435117?v=4",
        Url = "https://github.com/bubbles-bot-gh",
        Type = "organization"
    };
    
    // ActionOptions related settings
    public static readonly string PostComment = "always";
    public static readonly bool AutoMerge = true;
    public static readonly string CustomCommand = "/fastforward";
    
    public static string RepoUrl => "https://github.com/LuneiSolei/Fast-Forward-Blossom-Bot-Tests.git";

    internal static Mock<IGitHubApiCallerFactory> CreateMockGitHubApiCallerFactory()
    {
        // Create new mock
        Mock<IGitHubApiCallerFactory> mockFactory = new();
        
        // Set up mock
        mockFactory
            .Setup(factory => factory.Create())
            .Returns(Mock.Of<IGitHubApiCaller>());

        return mockFactory;
    }

    internal static Mock<IRepoInfo> CreateMockRepoInfo()
    {
        // Create mock
        Mock<IRepoInfo> mockRepoInfo = new();
        
        // Set up mock
        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.Owner)
            .Returns(EventInfoFixture.Owner);
        
        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.Name)
            .Returns(EventInfoFixture.Name);

        mockRepoInfo
            .SetupGet(repoInfo => repoInfo.CloneUrl)
            .Returns(EventInfoFixture.CloneUrl);

        return mockRepoInfo;
    }

    internal static Mock<IEventInfo> CreateMockEventInfo()
    {
        // Create mock
        Mock<IEventInfo> mockEventInfo = new();
        
        // Set up mock
        mockEventInfo
            .SetupGet(eventInfo => eventInfo.User)
            .Returns(MockUser.Login);

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.UserHasPerms)
            .Returns(() => Task.FromResult(true));

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.CommandInvoked)
            .Returns(true);

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.CommentBody)
            .Returns("An example comment body");

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.IsPossible)
            .Returns(true);

        mockEventInfo
            .SetupGet(eventInfo => eventInfo.ShouldExit)
            .Returns(false);

        return mockEventInfo;
    }

    // internal static PullRequest CreatePullRequestResponse()
    // {
    //     // TODO: Is this actually the api response or is this the webhook event?
    //
    //
    //     return new PullRequest();
    // }

    public static PullRequestOpenedEvent GetPullRequestOpenedEvent()
    {
        string file = File.ReadAllText(PrOpenedEventJson);
        PullRequestOpenedEvent eventData = JsonSerializer.Deserialize<PullRequestOpenedEvent>(file) 
            ?? throw new InvalidOperationException();
        
        Console.WriteLine(PrOpenedEventJson);
        
        return eventData;
    }

    internal static IssueCommentCreatedEvent CreateIssueCommentCreatedEvent()
    {
        return new IssueCommentCreatedEvent
        {
            Issue = new()
            {
                Number = 123,
                Url = "some-fake-url",
                RepositoryUrl = "some-fake-repo-url",
                LabelsUrl = "some-fake-labels-url",
                CommentsUrl = "some-fake-comments-url",
                EventsUrl = "some-fake-events-url",
                HtmlUrl = "some-fake-html-url",
                NodeId = "123abc",
                Title = "An Example Issue Title",
                User = MockUser,
                Assignees = []
            },
            Comment = new IssueComment
            {
                Url = "some-fake-url",
                HtmlUrl = "some-fake-html-url",
                IssueUrl = "some-fake-issue-url",
                NodeId = "456def",
                User = MockUser,
                AuthorAssociation = AuthorAssociation.Collaborator,
                Body = "Some comment body."
            }
        };
    }
}