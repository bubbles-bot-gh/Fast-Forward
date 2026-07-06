using BubblesBotGitHub.FastForward.Core.Git;

namespace BubblesBotGitHub.FastForward.Implements.Git;

internal sealed class ProcessOutFactory : IProcessOutFactory
{
    public IProcessOut Create(int exitCode, string stdOut, string stdErr)
    {
        return new ProcessOut(exitCode, stdOut, stdErr);
    }
}