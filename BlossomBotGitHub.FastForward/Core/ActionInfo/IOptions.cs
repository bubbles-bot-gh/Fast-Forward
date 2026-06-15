namespace BlossomBotGitHub.FastForward.Core.ActionInfo;

public interface IOptions
{
    bool AutoMerge { get; }
    string PostComment { get; }
    string CustomCommand { get; }
}