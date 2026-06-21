namespace BlossomBotGitHub.FastForward.Core.Git;

internal interface IProcessOut
{
    public int ExitCode { get; }
    public string StdOut { get; }
    public string StdErr { get; }
}