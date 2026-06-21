namespace BlossomBotGitHub.FastForward.Core.Errors;

internal abstract class CustomException : Exception
{
    public int Line { get; }
    public string Reason { get; }
    
    protected CustomException(string msg, string reason, string sourceFile, int sourceLine) :
        base(FormatMessage(msg, reason, sourceFile, sourceLine))
    {
        base.Source = sourceFile;
        Line = sourceLine;
        Reason = reason;
        
        Console.Error.WriteLine($"::error file={sourceFile},line={sourceLine}::{base.Message}");
    }

    private static string FormatMessage(string msg, string reason, string sourceFile, int sourceLine) =>
        $"{msg} at {sourceFile}:{sourceLine}. \nReason: {reason}";
}