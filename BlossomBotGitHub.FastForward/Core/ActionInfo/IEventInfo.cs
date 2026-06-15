using Octokit.Webhooks;

namespace BlossomBotGitHub.FastForward.Core.ActionInfo;

public delegate Task<bool> UserPermsCheck(IRepoInfo repo);
public delegate Task<bool> IsPossible(IRepoInfo repo);

public interface IEventInfo
{
    IApiCaller ApiCaller { set; }
    bool ShouldExit { get; set; }
    UserPermsCheck GetUserHasPerms { get; set; }
    IsPossible GetIsPossible { get; set; }
    string CommentBody { get; }
    bool CommandInvoked { get; }
    ActionEventType EventType { get; }
    WebhookEvent Event { get; }
    string User { get; }
}