using BubblesBotGitHub.FastForward.Application.Git;
using BubblesBotGitHub.FastForward.Core.Enums;
using BubblesBotGitHub.Tests.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;

namespace BubblesBotGitHub.Tests.Fixtures;

public class GitTestFixture
{
    public string TempDirectory => AssemblyFixture.TestTempDirectory;
    public string CloneUrl => AssemblyFixture.TestRepoUrl;
    public string ExcludeCommit => "8fbc7d5cd07170344e7d3404622f7f987163655a";
    public string BaseSha => AssemblyFixture.TestRepoBaseSha;
    public string HeadSha => AssemblyFixture.TestRepoHeadSha;
    public string InvalidSha => "i-am-an-invalid-sha";
    public string RunnerTempEnvName => "RUNNER_TEMP";

    private readonly WebhookEvent _webhookEvent = AssemblyFixture.GetPullRequestOpenedEvent();
    private readonly ActionEventType _eventType = ActionEventType.PullRequestOpened;
    
    public static TheoryData<WebhookEvent, ActionEventType> TheoryData = AssemblyFixture.TheoryData;
    
    public HostApplicationBuilder CreateApplicationBuilderWithMocks(WebhookEvent webhookEvent, 
        ActionEventType eventType, MockServices serviceMocks) => 
        AssemblyFixture.CreateApplicationBuilderWithMocks(webhookEvent, eventType, serviceMocks);

    public IGit BuildGit()
    {
        IHost host = AssemblyFixture.CreateApplicationBuilderWithMocks(
            _webhookEvent,
            _eventType,
            new MockServices
            {
                MockGit = null,
                MockProcessOutFactory = null
            }).Build();
        return host.Services.GetRequiredService<IGit>();
    }
}