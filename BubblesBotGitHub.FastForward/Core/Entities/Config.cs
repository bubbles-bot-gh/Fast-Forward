namespace BubblesBotGitHub.FastForward.Core.Entities;

public class Config
{
    public required string IdRequestTokenEnvName { get; init; }
    public required string IdRequestUrlEnvName { get; init; }
    public required string SupabaseRequestTokenUrl { get; init; }
}