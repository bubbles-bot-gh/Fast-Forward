using BlossomBotGitHub.Tests;

[assembly: AssemblyFixture(typeof(TestFixture))]
namespace BlossomBotGitHub.Tests;

public class TestFixture
{
    public void TestFunc(ITestOutputHelper testOut)
    {
        testOut.WriteLine("Hello World!");
    }
}