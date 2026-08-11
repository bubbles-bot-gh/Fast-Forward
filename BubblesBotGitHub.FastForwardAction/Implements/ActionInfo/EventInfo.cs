using BubblesBotGitHub.FastForward.Core.ActionInfo;
using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal class EventInfo(WebhookEvent @event, IActionOptions opts, IGitHubApiCaller gitHubApiCaller) : IEventInfo
{
    public bool ShouldExit { get; set; } = false;
    public Task<bool> UserHasPerms => gitHubApiCaller.IsCollaborator("", "", "");
    public bool IsPossible { get; set; } = false;
    public string CommentBody => 
        (@event as PullRequestOpenedEvent)?.PullRequest.Body
        ?? (@event as IssueCommentCreatedEvent)?.Comment.Body 
        ?? string.Empty;
    public bool CommandInvoked => CommentBody.Trim() == opts.CustomCommand;
    public string User => @event.Sender?.Name ?? string.Empty;

    // public EventInfo(WebhookEvent @event, IActionOptions opts, IGitHubApiCaller githubApiCaller)
    // {
    //     GitHubApiCaller = githubApiCaller;
    //     IsPossible = false;
    //     CommentBody = (@event as PullRequestOpenedEvent)?.PullRequest.Body ?? string.Empty;
    //     CommandInvoked = CommentBody.Trim() == opts.CustomCommand;
    //     User = @event.Sender?.Name ?? string.Empty;
    // }
}