using BubblesBotGitHub.FastForward.Application.ActionInfo;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal sealed record ActionOptions : IActionOptions
{
    private string IsAutoMergeField
    {
        init
        {
            List<string> validValues = ["true", "false"];
            if (!validValues.Contains(value)) throw new InvalidOperationException("'auto_merge' must be either 'true' or 'false'");

            IsAutoMerge = value == "true";
        }
    }

    public bool IsAutoMerge { get; private init; }
    public string CustomCommand { get; }
    public string PostComment
    {
        get;
        private init
        {
            List<string> validValues = ["always", "on-error", "never"];
            if (!validValues.Contains(value)) throw new InvalidOperationException($"Cannot assign value '{value}' to "
                + "ActionOptions.PostComment. Valid values are 'always', 'on-error', or 'never'.");

            field = value;
        }
    }

    public ActionOptions()
    {
        IsAutoMergeField = Environment.GetEnvironmentVariable("INPUT_AUTO_MERGE") ?? "false";
        CustomCommand = Environment.GetEnvironmentVariable("INPUT_CUSTOM_COMMAND") ?? "/fast-forward";
        PostComment = Environment.GetEnvironmentVariable("INPUT_POST_COMMENT") ?? "on-error";
    }
}