namespace BubblesBotGitHub.Tests.Fixtures.ServiceCollectionExtensionTests;

public static class ServiceCollectionExtensionsFixture
{
    public const string IsAutoMergeEnvName = "INPUT_AUTO_MERGE";
    public const string CustomCommandEnvName = "INPUT_CUSTOM_COMMAND";
    public const string PostCommentEnvName = "INPUT_POST_COMMENT";
    public const bool IsAutoMerge = true;
    public const string CustomCommand = "/custom-command";
    public const string PostComment = "always";
}