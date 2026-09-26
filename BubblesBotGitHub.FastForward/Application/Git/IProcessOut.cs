namespace BubblesBotGitHub.FastForward.Application.Interfaces;

public interface IProcessOut
{
    public int ExitCode { get; }
    public string StdOut { get; }
    public string StdErr { get; }
}