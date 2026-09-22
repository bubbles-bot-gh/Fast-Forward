using System.Runtime.CompilerServices;

namespace BubblesBotGitHub.FastForward.Core.Errors;

internal sealed class OctokitInitException(
    string reason, 
    [CallerFilePath] string sourceFile = "", 
    [CallerLineNumber] int sourceLine = 0
) : CustomException(FormatMessage(), reason, sourceFile, sourceLine)
{
    private static string FormatMessage() =>
        "Something went wrong while trying to initialize an instance of Octokit";
}