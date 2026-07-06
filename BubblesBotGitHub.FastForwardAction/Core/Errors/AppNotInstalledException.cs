using System.Runtime.CompilerServices;

namespace BubblesBotGitHub.FastForward.Core.Errors;

internal sealed class AppNotInstalledException(
    string owner,
    string repoName,
    string reason,
    [CallerFilePath] string sourceFile = "",
    [CallerLineNumber] int sourceLine = 0 
) : CustomException(FormatMessage(owner, repoName), reason, sourceFile, sourceLine)
{
    private static string FormatMessage(string owner, string repoName) =>
        $"The GitHub App for 'Fast-Forward-Blossom-Bot' is not installed in repository '{owner}/{repoName}'";
}