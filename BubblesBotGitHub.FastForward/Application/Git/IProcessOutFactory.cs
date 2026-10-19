namespace BubblesBotGitHub.FastForward.Application.Git;

public interface IProcessOutFactory
{
    IProcessOut Create(int exitCode, string stdOut, string stdErr);
}