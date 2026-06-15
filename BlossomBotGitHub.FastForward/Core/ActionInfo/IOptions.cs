namespace BlossomBotGitHub.FastForward.Core.ActionInfo;

internal interface IOptions
{
    bool AutoMerge { get; }
    string PostComment { get; }
    string CustomCommand { get; }
}