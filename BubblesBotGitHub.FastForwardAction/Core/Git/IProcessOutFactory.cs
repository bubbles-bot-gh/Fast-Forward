namespace BubblesBotGitHub.FastForward.Core.Git;

public interface IProcessOutFactory
{
    IProcessOut Create(int exitCode, string stdOut, string stdErr);
}