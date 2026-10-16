namespace BubblesBotGitHub.FastForward.Application.ActionInfo;

public interface IActionOptions
{
    bool IsAutoMerge { get; }
    string PostComment { get; }
    string CustomCommand { get; }
}