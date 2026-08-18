using BubblesBotGitHub.FastForward.Core.ActionInfo;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal sealed record ActionOptions : IActionOptions
{    
    public bool IsAutoMerge { get; }
    public string CustomCommand { get; }
    
    public string PostComment
    {
        get;
        private init
        {
            List<string> validValues = ["always", "on-error", "never"];
            if (!validValues.Contains(value)) return;

            field = value;
        }
    }

    public ActionOptions()
    {
        IsAutoMerge = Environment.GetEnvironmentVariable("INPUT_AUTO_MERGE") == "true";
        CustomCommand = Environment.GetEnvironmentVariable("INPUT_CUSTOM_COMMAND") ?? "/fast-forward";
        PostComment = Environment.GetEnvironmentVariable("INPUT_POST_COMMENT") ?? "on-error";
    }
}