using BubblesBotGitHub.FastForward.Application.ActionInfo;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal sealed class EventInfo : IEventInfo
{
    public bool ShouldExit { get; set; } = false;
    public Task<bool> IsPossible => _isPossible.Value;
    private readonly Lazy<Task<bool>> _isPossible;
    public bool CommandInvoked { get; }
    public Task<bool> UserHasPerms { get; }
    public string User { get; }
    public string CommentBody { get; }

    public EventInfo(
        WebhookEvent webhookEvent, 
        IActionOptions opts,
        IRepoInfo repoInfo,
        IPrInfo prInfo,
        IGitHubClient gitHubClient)
    {
        CommentBody = (webhookEvent as PullRequestOpenedEvent)?.PullRequest.Body
            ?? (webhookEvent as IssueCommentCreatedEvent)?.Comment.Body 
            ?? (webhookEvent as IssueCommentEditedEvent)?.Comment.Body
            ?? string.Empty;
        CommandInvoked = CommentBody.Trim() == opts.CustomCommand;
        User = webhookEvent.Sender?.Login ?? string.Empty;
        UserHasPerms = gitHubClient.IsCollaboratorAsync(repoInfo.Owner, repoInfo.Name, User);
        _isPossible = new Lazy<Task<bool>>(async () =>
        {
            CompareResult result = await gitHubClient.GetBaseHeadComparison(repoInfo.Owner, repoInfo.Name, prInfo.BaseSha, prInfo.HeadSha);
            
            return result.Status == "ahead";
        });
    }
}