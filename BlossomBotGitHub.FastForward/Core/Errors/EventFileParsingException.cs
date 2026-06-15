using System.Runtime.CompilerServices;

namespace BlossomBotGitHub.FastForward.Core.Errors;

internal sealed class EventFileParsingException(
        string eventPath, 
        string reason, 
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int sourceLine = 0
) : CustomException(FormatMessage(eventPath), reason, sourceFile, sourceLine)
{
    private static string FormatMessage(string eventPath) =>
        $"Event file could not be parsed for '{eventPath}'";
}