namespace BubblesBotGitHub.FastForward.Application.Interfaces;

public interface IActionOptions
{
    bool IsAutoMerge { get; }
    string PostComment { get; }
    string CustomCommand { get; }
}