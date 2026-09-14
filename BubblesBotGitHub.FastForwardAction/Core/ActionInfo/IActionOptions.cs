namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

public interface IActionOptions
{
    bool IsAutoMerge { get; }
    string PostComment { get; }
    string CustomCommand { get; }
}