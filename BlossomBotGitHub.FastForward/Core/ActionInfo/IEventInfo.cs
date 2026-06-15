using Octokit.Webhooks;

namespace BlossomBotGitHub.FastForward.Core.ActionInfo;

internal delegate Task<bool> UserPermsCheck(IRepoInfo repo);
internal delegate Task<bool> IsPossible(IRepoInfo repo);

internal interface IEventInfo
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