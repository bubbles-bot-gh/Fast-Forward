namespace BlossomBotGitHub.FastForward.Core;

public interface ICommit
{
    public interface IRepository
    {
        public interface IObject
        {
            public string Oid { get; }
            public string Message { get; }
            public string CommittedDate { get; }

            public interface IAuthor
            {
                public string Name { get; }
                public string Email { get; }
            }

            public interface IAssociatedPullRequests
            {
                public interface INodes
                {
                    public string HeadRefName { get; }
                    public string BaseRefName { get; }
                }
            }
        }
    }
}