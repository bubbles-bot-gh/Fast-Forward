namespace BubblesBotGitHub.FastForward.Application.ActionInfo;

public interface IEventInfo
{
    bool ShouldExit { get; set; }
    Task<bool> UserHasPerms { get; }
    Task<bool> IsPossible { get; }
    string CommentBody { get; }
    bool CommandInvoked { get; }
    string User { get; }
}