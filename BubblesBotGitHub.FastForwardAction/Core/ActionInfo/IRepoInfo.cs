namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IRepoInfo
{
    string Name { get; }
    string Owner { get; }
    string CloneUrl { get; }
}