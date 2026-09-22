using BubblesBotGitHub.FastForward;

namespace BubblesBotGitHub.Tests.Integration.Tests;

public class ProgramTests
{
    [Fact]
    public async Task SucceedsRunning()
    {
        await Program.Main();
    }
}