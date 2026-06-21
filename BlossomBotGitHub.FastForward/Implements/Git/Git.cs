using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BlossomBotGitHub.FastForward.Core.Errors;
using BlossomBotGitHub.FastForward.Core.Git;

namespace BlossomBotGitHub.FastForward.Implements.Git;

internal class Git(IProcessOutFactory processOutFactory) : IGit
{
    private async Task<IProcessOut> RunProcessAsync(
        string fileName,
        IEnumerable<string> args,
        string workingDir)
    {
        using Process proc = StartProcess(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = string.Join(' ', args.Select(QuoteArg)),
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        
        Task<string> outputTask = proc.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();
        
        return processOutFactory.Create(
            exitCode: proc.ExitCode,
            stdOut: await outputTask,
            stdErr: await errorTask);

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
        if (workingDir == string.Empty) throw new ArgumentException(workingDir);
        Console.WriteLine($"::info::Cloning repo from '{cloneUrl}' into '{workingDir}'");
        
        // Make sure arguments are valid
        if (string.IsNullOrWhiteSpace(cloneUrl)) throw new ArgumentException(null, nameof(cloneUrl));

        // Delete and recreate working directory, if it already exists
        if (Directory.Exists(workingDir))
        {
            foreach (string entry in Directory.EnumerateFileSystemEntries(workingDir))
                DeleteRecursive(entry);
        }

        Directory.CreateDirectory(workingDir);
        
        // Perform cloning
        IProcessOut result = await RunProcessAsync(
            fileName: "git",
            args: ["clone", cloneUrl, "."], 
            workingDir: workingDir);
        
        if (result.ExitCode != 0) throw new GitCommandException( $"git clone {cloneUrl} .", result.StdErr);
    }

    public async Task<string> Log(string exclude, string baseSha, string headSha, string workingDir)
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
            ],
            workingDir);
        
        return result.ExitCode != 0 ? throw new GitCommandException(
            $"git log --pretty=oneline --graph {exclude} {baseSha} {headSha}", result.StdErr) : result.StdOut;
    }

    public async Task<string> GetMergeBaseSha(string baseSha, string headSha, string workingDir)
    {
        IProcessOut result = await RunProcessAsync("git", ["merge-base", baseSha, headSha], workingDir);
        
        return result.ExitCode != 0 ? throw new GitCommandException(
            $"git merge-base {baseSha} {headSha}" ,result.StdErr) : result.StdOut.Trim();
    }

    public async Task<uint> GetAmountOfParents(string sha, string workingDir)
    {
        IProcessOut result = await RunProcessAsync(
            fileName: "git", 
            args: ["log", "--parents", "-n", "1", sha],
            workingDir);
        if (result.ExitCode != 0) throw new GitCommandException(
            $"git log --parents -n 1 {sha}", result.StdErr);

        string first = result.StdOut
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)[0];
        List<string> parts = first
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        
        // parts[0] == 'commit', parts[1] == self sha
        uint parents = (uint)parts.Count - 2;
        
        return parents;
    }
}