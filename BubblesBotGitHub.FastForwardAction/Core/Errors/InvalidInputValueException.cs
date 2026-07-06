using System.Runtime.CompilerServices;

namespace BubblesBotGitHub.FastForward.Core.Errors;

internal sealed class InvalidInputValueException(
    string varName,
    string varValue,
    string reason,
    [CallerFilePath] string sourceFile = "",
    [CallerLineNumber] int sourceLine = 0
) : CustomException(FormatMessage(varName, varValue), reason, sourceFile, sourceLine)
{
    private static string FormatMessage(string varValue, string varName) =>
        $"Invalid value '{varValue}' for workflow input '{varName}'";
}