namespace BubblesBotGitHub.FastForward.Core.ActionInfo;

internal interface IRepoInfo
{
    string Name { get; }
    IPrInfo Pr { get; }
    string Owner { get; }
    string CloneUrl { get; }
}