using BubblesBotGitHub.FastForward.Core.GitHubApiCaller;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

public interface IActionInfoFactory
{
    Task<IActionInfo> Create();
}