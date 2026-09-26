namespace BubblesBotGitHub.FastForward.Application.Interfaces;

public interface IRepoInfo
{
    string Name { get; }
    string Owner { get; }
    string CloneUrl { get; }
}