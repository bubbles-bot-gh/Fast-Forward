namespace BlossomBotGitHub.FastForward.Core.GraphQL;

public interface ICollaboratorResponse
{
    public interface IRepository
    {
        public interface ICollaborators
        {
            public int TotalCount { get; }
        }
    }
}