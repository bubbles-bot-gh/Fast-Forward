namespace BubblesBotGitHub.FastForward.Core.Entities;

public class Config
{
    public required string IdRequestTokenEnvName { get; init; }
    public required string IdRequestUrlEnvName { get; init; }
    public required string SupabaseRequestTokenUrl { get; init; }
    public required string SupabasePublishableKey { get; init; }
    public required string Audience { get; init; }
    
    // Keep init for testing validation
    public string WorkingDir { get; init; } = Environment.GetEnvironmentVariable("RUNNER_TEMP") ?? Path.GetTempPath();
}