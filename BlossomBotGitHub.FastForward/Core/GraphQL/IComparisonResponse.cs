namespace BlossomBotGitHub.FastForward.Core.GraphQL;

public interface IComparisonResponse
{
    public int Status { get; }
    public Dictionary<string, string> Headers { get; }

    public interface IData
    {
        public string Status { get; }
    }
}