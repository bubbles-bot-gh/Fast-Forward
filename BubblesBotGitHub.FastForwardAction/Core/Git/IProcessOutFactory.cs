namespace BubblesBotGitHub.FastForward.Core.Git;

internal interface IProcessOutFactory
{
    IProcessOut Create(int exitCode, string stdOut, string stdErr);
}