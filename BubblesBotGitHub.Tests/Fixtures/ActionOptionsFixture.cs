using System.Diagnostics.CodeAnalysis;
using BubblesBotGitHub.FastForward.Core;
using BubblesBotGitHub.FastForward.Core.Enums;
using JetBrains.Annotations;
using Octokit.Webhooks.Events.PullRequest;

namespace BubblesBotGitHub.Tests.Fixtures;

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[UsedImplicitly]
public class ActionOptionsFixture : IAsyncLifetime
{
    private const string AutoMergeEnvName = "INPUT_AUTO_MERGE";
    private const string PostCommentEnvName = "INPUT_POST_COMMENT";
    private const string CustomCommandEnvName = "INPUT_CUSTOM_COMMAND";
    public readonly bool AutoMergeValue = true;
    public readonly string CustomCommandValue = "/fast-forward";
    public readonly string PostCommentValue = "always";
    public readonly PullRequestOpenedEvent WebhookEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    public readonly ActionEventType EventType = ActionEventType.PullRequestOpened;
    
    public ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable(AutoMergeEnvName, AutoMergeValue.ToString().ToLowerInvariant());
        Environment.SetEnvironmentVariable(CustomCommandEnvName, CustomCommandValue);
        Environment.SetEnvironmentVariable(PostCommentEnvName, PostCommentValue);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Environment.SetEnvironmentVariable(AutoMergeEnvName, null);
        Environment.SetEnvironmentVariable(CustomCommandEnvName, null);
        Environment.SetEnvironmentVariable(PostCommentEnvName, null);
        
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}