namespace BubblesBotGitHub.FastForward.Application;

public interface ICommentBuilder
{
    public Task<string> Build();
}