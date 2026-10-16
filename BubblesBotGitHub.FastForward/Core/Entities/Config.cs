namespace BubblesBotGitHub.FastForward.Core.Entities;

public class Config
{
    public required string IdRequestTokenEnvName { get; init; }
    public required string IdRequestUrlEnvName { get; init; }
    public required string SupabaseRequestTokenUrl { get; init; }
    public required string SupabasePublishableKey { get; init; }
    public required string Audience { get; init; }
    public string WorkingDir { get; } = Environment.GetEnvironmentVariable("RUNNER_TEMP") ?? Path.GetTempPath();
}