using BubblesBotGitHub.FastForward.Core.ActionInfo;

namespace BubblesBotGitHub.FastForward.Implements.ActionInfo;

internal class RepoInfo : IRepoInfo
{
    public string Name { get; }
    public string Owner { get; }
    public string CloneUrl { get; }
}