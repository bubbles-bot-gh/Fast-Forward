namespace BlossomBotGitHub.FastForward.Core.GraphQL;

internal interface IComparisonResponse
{
    internal int Status { get; }
    internal Dictionary<string, string> Headers { get; }

    internal interface IData
    {
        internal string Status { get; }
    }
}