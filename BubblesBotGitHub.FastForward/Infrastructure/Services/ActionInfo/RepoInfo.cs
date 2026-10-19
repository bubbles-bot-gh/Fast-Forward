using BubblesBotGitHub.FastForward.Application.ActionInfo;
using Octokit.Webhooks;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services.ActionInfo;

internal class RepoInfo(WebhookEvent webhookEvent) : IRepoInfo
{
    public string Name { get; } = webhookEvent.Repository?.Name ?? string.Empty;
    public string Owner { get; } = webhookEvent.Repository?.Owner.Login ?? string.Empty;
    public string CloneUrl { get; } = webhookEvent.Repository?.CloneUrl ?? string.Empty;
}