namespace BlossomBotGitHub.FastForward.Core.GraphQL;

public interface IPrResponse
{
    public interface IRepository 
    {
        public interface IPullRequest
        {
            public string BaseRefName { get; }
            public string BaseRefOid { get; }
            public string HeadRefName { get; }
            public string HeadRefOid { get; }
            public interface IHeadRepository
            {
                public string Name { get; }
            }
            public interface IHeadRepositoryOwner
            {
                public string Login { get; }
            }
            public string Id { get; }
        }
    }
}