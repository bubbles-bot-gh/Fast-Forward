using System.Net;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.Hosting;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

public class GitHubClientTestFixture
{
    public readonly string Owner = "bubbles-bot-gh";
    public readonly string Name = "Fast-Forward";
    public readonly string User = "luneisolei";
    public readonly string Comment = "This is an example comment.";
    public readonly int PullRequestNumber = 365;
    public readonly uint IssueNumber = 1958;
    public readonly string BaseSha = AssemblyFixture.TestRepoBaseSha;
    public readonly string HeadSha = AssemblyFixture.TestRepoHeadSha;
    
    public readonly NotFoundException OctokitException = new("missing", HttpStatusCode.NotFound);
    
    public PullRequestOpenedEvent GetPullRequestOpenedEvent => AssemblyFixture.GetPullRequestOpenedEvent();
    
    public readonly Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder> 
        CreateApplicationBuilderWithMocks = AssemblyFixture.CreateApplicationBuilderWithMocks;
}