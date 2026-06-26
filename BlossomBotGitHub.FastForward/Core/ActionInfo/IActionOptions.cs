namespace BlossomBotGitHub.FastForward.Core.ActionInfo;

internal interface IActionOptions
{
    bool IsAutoMerge { get; }
    string PostComment { get; }
    string CustomCommand { get; }
}