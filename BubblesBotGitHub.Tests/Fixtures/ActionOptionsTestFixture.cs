using System.Diagnostics.CodeAnalysis;
using BubblesBotGitHub.FastForward.Application.ActionInfo;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Fixtures;

[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[UsedImplicitly]
public class ActionOptionsTestFixture : IAsyncLifetime
{
    public string InputAutoMergeEnvName => "INPUT_AUTO_MERGE";
    public string PostCommentEnvName => "INPUT_POST_COMMENT";
    public string CustomCommandEnvName => "INPUT_CUSTOM_COMMAND";
    public WebhookEvent DummyWebhookEvent { get; private set; } = new Mock<WebhookEvent>().Object;
    public ActionEventType DummyEventType { get; private set; } = ActionEventType.PullRequestOpened;
    
    private readonly bool _autoMergeValue = true;
    private const string CustomCommandValue = "/fast-forward";
    private readonly string _postCommentValue = "always";
    
    public ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable(InputAutoMergeEnvName, _autoMergeValue.ToString().ToLowerInvariant());
        Environment.SetEnvironmentVariable(CustomCommandEnvName, CustomCommandValue);
        Environment.SetEnvironmentVariable(PostCommentEnvName, _postCommentValue);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Environment.SetEnvironmentVariable(InputAutoMergeEnvName, null);
        Environment.SetEnvironmentVariable(CustomCommandEnvName, null);
        Environment.SetEnvironmentVariable(PostCommentEnvName, null);
        
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData => AssemblyFixture.TheoryData;

    public static TheoryData<string> PostCommentInputValuesTheoryData =>
    [
        new TheoryDataRow<string>("always"),
        new TheoryDataRow<string>("on-error"),
        new TheoryDataRow<string>("never")
    ];

    public Func<WebhookEvent, ActionEventType, MockServices, HostApplicationBuilder>
        CreateApplicationBuilderWithMocks => AssemblyFixture.CreateApplicationBuilderWithMocks;

    public IActionOptions BuildActionOptions()
    {
        IHost host = CreateApplicationBuilderWithMocks(
            DummyWebhookEvent,
            DummyEventType,
            new MockServices
            {
                MockActionOptions = null
            }).Build();

        return host.Services.GetRequiredService<IActionOptions>();
    }
}