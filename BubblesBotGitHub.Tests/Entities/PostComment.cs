namespace BubblesBotGitHub.Tests.Entities;

public enum PostComment
{
    Always,
    Never,
    OnError
}

public static class EnumExtensions
{
    extension(PostComment value)
    {
        public string ToStringValue() => value switch
        {
            PostComment.Always  => "always",
            PostComment.Never   => "never",
            PostComment.OnError => "on-error",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }
}