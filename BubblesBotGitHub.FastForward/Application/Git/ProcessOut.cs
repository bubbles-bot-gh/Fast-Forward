using BubblesBotGitHub.FastForward.Application.Interfaces;

namespace BubblesBotGitHub.FastForward.Application;

internal class ProcessOut(int exitCode, string stdOut, string stdErr) : IProcessOut
{
    public int ExitCode { get; } = exitCode;
    public string StdOut { get; } = stdOut;
    public string StdErr { get; } = stdErr;
}