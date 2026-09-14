namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

public interface IRepoInfo
{
    string Name { get; }
    string Owner { get; }
    string CloneUrl { get; }
}