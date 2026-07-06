namespace BubblesBotGitHub.FastForward.Core.GraphQL;

internal interface IPrResponse
{
    internal interface IRepository 
    {
        internal interface IPullRequest
        {
            internal string BaseRefName { get; }
            internal string BaseRefOid { get; }
            internal string HeadRefName { get; }
            internal string HeadRefOid { get; }
            internal interface IHeadRepository
            {
                internal string Name { get; }
            }
            public interface IHeadRepositoryOwner
            {
                internal string Login { get; }
            }
            internal string Id { get; }
        }
    }
}