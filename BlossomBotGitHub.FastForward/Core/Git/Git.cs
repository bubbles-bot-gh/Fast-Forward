using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BlossomBotGitHub.FastForward.Implements;

namespace BlossomBotGitHub.FastForward.Core.Git;

internal class Git : IGit
{
    private const string RepoWd = "./tmp/repo";
    private IProcessOut? _processOut;

    private async Task<IProcessOut> RunProcessAsync(
        string fileName,
        IEnumerable<string> args,
        string workingDirectory = ".")
    {
        using Process proc = StartProcess(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = string.Join(' ', args.Select(QuoteArg)),
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        
        Task<string> outputTask = proc.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();
        
        // TODO: use DI to inject the concrete type for ProcessOut. Inject a ProcessOutFactory into this class and use that to 
        //  build new instance(s) of ProcessOut
        return new ProcessOut(
            ExitCode: proc.ExitCode,
            StdOut: await outputTask,
            StdErr: await errorTask);

        // Helper to avoid quoting issues
        static string QuoteArg(string a) =>
            a.Contains(' ') || a.Contains('"') ? "\"" + a.Replace("\"", "\\\"") + "\"" : a;
        
        // Helper to start process
        [ExcludeFromCodeCoverage(Justification = "Unnecessary check")]
        static Process StartProcess(ProcessStartInfo startInfo) =>
            Process.Start(startInfo) ?? throw new NullReferenceException("Failed to start process");
    }

    private void DeleteRecursive(string path)
    {
        if (Directory.Exists(path)) { Directory.Delete(path, recursive: true); }
        else if (File.Exists(path)) { File.Delete(path); }
    }

    public async Task CloneRepoAsync(string cloneUrl, string workingDir)
    {
        Console.WriteLine($"::info::Cloning repo from '{cloneUrl}'");
        
        // Make sure arguments are valid
        if (string.IsNullOrWhiteSpace(cloneUrl)) throw new ArgumentException(null, nameof(cloneUrl));
        if (string.IsNullOrWhiteSpace(workingDir)) throw new ArgumentException(null, nameof(workingDir));

        // Delete and recreate ./tmp/repo
        string repoDir = Path.Combine(workingDir, "repo");
        if (Directory.Exists(repoDir))
        {
            foreach (string entry in Directory.EnumerateFileSystemEntries(repoDir))
                DeleteRecursive(entry);
        }

        Directory.CreateDirectory(repoDir);
        
        // Perform cloning
        IProcessOut result = await RunProcessAsync(
            "git", ["clone", cloneUrl, "."], repoDir);
        
        if (result.ExitCode != 0) throw new Exception($"git clone failed: {result.StdErr}");
    }

    public async Task<string> GetMergeBaseSha(string exclude, string baseSha, string headSha)
    {
        IProcessOut result = await RunProcessAsync(
            fileName: "git",
            args: [
                "log",
                "--pretty=oneline",
                "--graph",
                exclude,
                baseSha,
                headSha
            ]);
        
        return result.ExitCode != 0 ? throw new Exception($"git error: {result.StdErr}") : result.StdOut;
    }

    public async Task<string> GetMergeBaseSha(string baseSha, string headSha)
    {
        IProcessOut result = await RunProcessAsync("git", ["merge-base", baseSha, headSha]);
        return result.ExitCode != 0 ? throw new Exception($"git error: {result.StdErr}") : result.StdOut;
    }

    public async Task<uint> GetAmountOfParents(string sha)
    {
        
        IProcessOut result = await RunProcessAsync(
            fileName: "git", 
            args: ["log", "--parents", "-n", "1", sha],
            workingDirectory: RepoWd);
        if (result.StdOut.Length == 0) return 0;

        List<string> parts = result.StdOut.Split(' ').ToList();
        uint parents = (uint)parts.Count - 1;
        
        return result.ExitCode != 0 ? throw new Exception($"git error: {result.StdErr}") : parents;
    }
}