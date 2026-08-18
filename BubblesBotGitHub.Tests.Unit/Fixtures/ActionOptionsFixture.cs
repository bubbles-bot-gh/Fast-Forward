using System.Diagnostics.CodeAnalysis;

namespace BubblesBotGitHub.Tests.Unit.Fixtures;

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
public static class ActionOptionsFixture
{
    public static readonly string AutoMergeEnvName = "INPUT_AUTO_MERGE";
    public static readonly string PostCommentEnvName = "INPUT_POST_COMMENT";
    public static readonly string CustomCommandEnvName = "INPUT_CUSTOM_COMMAND";
}