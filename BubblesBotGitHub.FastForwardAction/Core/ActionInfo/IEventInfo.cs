namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal delegate Task<bool> UserPermsCheck(IRepoInfo repo);
internal delegate Task<bool> IsPossible(IRepoInfo repo);

internal interface IEventInfo
{
    bool ShouldExit { get; set; }
    Task<bool> UserHasPerms { get; }
    bool IsPossible { get; set; }
    string CommentBody { get; }
    bool CommandInvoked { get; }
    string User { get; }
}