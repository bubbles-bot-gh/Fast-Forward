namespace BlossomBotGitHub.FastForward.Implements;

internal record ProcessOut(int ExitCode, string StdOut, string StdErr): Core.IProcessOut
{
    public int ExitCode { get; } = ExitCode;
    public string StdOut { get; } = StdOut;
    public string StdErr { get; } = StdErr;
}