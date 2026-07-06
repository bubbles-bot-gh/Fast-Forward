using BubblesBotGitHub.FastForward.Core.Git;

namespace BubblesBotGitHub.FastForward.Implements.Git;

internal class ProcessOut(int exitCode, string stdOut, string stdErr) : IProcessOut
{
    public int ExitCode { get; } = exitCode;
    public string StdOut { get; } = stdOut;
    public string StdErr { get; } = stdErr;
}