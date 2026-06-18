namespace BlossomBotGitHub.FastForward.Core;

internal interface IProcessOut
{
    public int ExitCode { get; }
    public string StdOut { get; }
    public string StdErr { get; }
}