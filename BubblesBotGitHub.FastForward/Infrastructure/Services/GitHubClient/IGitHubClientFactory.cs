namespace BubblesBotGitHub.FastForward.Infrastructure.Services;

public interface IGitHubClientFactory
{
    public IGitHubClient Create();
}