using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IActionInfoFactory
{
    IActionInfo Create(WebhookEvent webhookEvent);
}