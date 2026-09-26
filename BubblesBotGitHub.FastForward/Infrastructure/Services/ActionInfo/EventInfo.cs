using BubblesBotGitHub.FastForward.Application.Interfaces;
using BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal sealed class EventInfo : IEventInfo
{
    public bool ShouldExit { get; set; } = false;
    public bool IsPossible { get; set; } = false;
    public bool CommandInvoked { get; }
    public Task<bool> UserHasPerms { get; }
    public string User { get; }
    public string CommentBody { get; }

    public EventInfo(
        WebhookEvent webhookEvent, 
        IActionOptions opts,
        IRepoInfo repoInfo,
        IGitHubClient gitHubClient)
    {
        CommentBody = (webhookEvent as PullRequestOpenedEvent)?.PullRequest.Body
            ?? (webhookEvent as IssueCommentCreatedEvent)?.Comment.Body 
            ?? string.Empty;
        CommandInvoked = CommentBody.Trim() == opts.CustomCommand;
        User = webhookEvent.Sender?.Login ?? string.Empty;
        UserHasPerms = gitHubClient.IsCollaborator(repoInfo.Owner, repoInfo.Name, User);
    }
}