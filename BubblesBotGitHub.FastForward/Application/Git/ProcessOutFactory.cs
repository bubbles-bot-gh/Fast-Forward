using BubblesBotGitHub.FastForward.Application.Interfaces;

namespace BubblesBotGitHub.FastForward.Application;

internal sealed class ProcessOutFactory : IProcessOutFactory
{
    public IProcessOut Create(int exitCode, string stdOut, string stdErr)
    {
        return new ProcessOut(exitCode, stdOut, stdErr);
    }
}