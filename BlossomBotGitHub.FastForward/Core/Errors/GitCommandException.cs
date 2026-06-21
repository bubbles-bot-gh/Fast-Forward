using System.Runtime.CompilerServices;

namespace BlossomBotGitHub.FastForward.Core.Errors;

internal sealed class GitCommandException(
    string msg,
    string stdErr, 
    [CallerFilePath] string sourceFile = "",
    [CallerLineNumber] int sourceLine = 0) : CustomException(FormatMessage(msg), stdErr, sourceFile, sourceLine)
{
    private static string FormatMessage(string msg) =>
        $"Git command '{msg}' failed";
}