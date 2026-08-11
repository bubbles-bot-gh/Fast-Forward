using BubblesBotGitHub.FastForward.Core.ActionInfo;

namespace BubblesBotGitHub.FastForward.Implements;

internal record ActionOptions : IActionOptions
{    
    public bool IsAutoMerge { get; }
    public string CustomCommand { get; }
    public string PostComment { get; }
    
    public ActionOptions()
    {
        IsAutoMerge = Environment.GetEnvironmentVariable("INPUT_AUTO_MERGE") == "true";
        PostComment = Environment.GetEnvironmentVariable("INPUT_POST_COMMENT") ?? "on-error";
        CustomCommand = Environment.GetEnvironmentVariable("INPUT_CUSTOM_COMMAND") ?? "/fast-forward";
    }
}