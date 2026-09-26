namespace BubblesBotGitHub.FastForward.Application.Interfaces;

public interface IProcessOutFactory
{
    IProcessOut Create(int exitCode, string stdOut, string stdErr);
}