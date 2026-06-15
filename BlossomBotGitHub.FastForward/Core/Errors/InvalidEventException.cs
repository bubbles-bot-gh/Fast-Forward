using System.Runtime.CompilerServices;

namespace BlossomBotGitHub.FastForward.Core.Errors;

internal sealed class InvalidEventException(
    string eventData,
    string reason,
    [CallerFilePath] string sourceFile = "",
    [CallerLineNumber] int sourceLine = 0
) : CustomException(FormatMessage(eventData), reason, sourceFile, sourceLine)
{
    private static string FormatMessage(string eventData) =>
        $"Received an invalid event with data: '{eventData}'";
}