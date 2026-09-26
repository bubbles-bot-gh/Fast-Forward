using BubblesBotGitHub.FastForward.Application.Interfaces;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal class RepoInfo : IRepoInfo
{
    public string Name { get; }
    public string Owner { get; }
    public string CloneUrl { get; }
}