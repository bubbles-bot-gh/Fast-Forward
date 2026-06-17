using System.Diagnostics;

namespace BlossomBotGitHub.FastForward.Core;

internal static class Git
{
    private static readonly string RepoWd = "./tmp/repo";

    private static async Task<(int ExitCode, string StdOut, string StdErr)> RunProcessAsync(
        string fileName,
        IEnumerable<string> args,
        string workingDirectory = ".")
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = fileName,
            Arguments = string.Join(' ', args.Select(QuoteArg)),
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using Process proc = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start process");
        Task<string> outputTask = proc.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();
        
        return (proc.ExitCode, await outputTask, await errorTask);

        static string QuoteArg(string a) =>
            a.Contains(' ') || a.Contains('"') ? "\"" + a.Replace("\"", "\\\"") + "\"" : a;
    }

    private static void TryDeleteRecursive(string path)
    {
        try
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
                Directory.Delete(path, recursive: true);
            else
                File.Delete(path);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"::error::{e.Message}");
        }
    }

    private static async Task<string> Exec(List<string> args, string cwd = "./")
    {
        string fileName = args[0];
        args.RemoveAt(0);
        
        ProcessStartInfo startInfo = new()
        {
            FileName = fileName,
            Arguments = string.Join(" ", args),
            WorkingDirectory = cwd,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        Process? proc = Process.Start(startInfo);

        ArgumentNullException.ThrowIfNull(proc);

        string output = await proc.StandardOutput.ReadToEndAsync();
        await proc.WaitForExitAsync();

        return output;
    }

    public static async Task CloneRepoAsync(string cloneUrl, string workingDir)
    {
        Console.WriteLine($"::info::Cloning repo from '{cloneUrl}'");
        if (string.IsNullOrWhiteSpace(cloneUrl)) throw new ArgumentException(null, nameof(cloneUrl));
        if (string.IsNullOrWhiteSpace(workingDir)) throw new ArgumentException(null, nameof(workingDir));

        string repoDir = Path.Combine(workingDir, "repo");
        if (Directory.Exists(repoDir))
        {
            foreach (string entry in Directory.EnumerateFileSystemEntries(repoDir))
                TryDeleteRecursive(entry);
        }

        Directory.CreateDirectory(repoDir);
        (int exitCode, string stdOut, string stdErr) = await RunProcessAsync("git", ["clone", cloneUrl, "."], repoDir);
        Console.WriteLine($"stdErr: {stdErr}");
        Console.WriteLine($"exitCode: {exitCode}");
        if (exitCode != 0) throw new Exception($"git clone failed: {stdErr}");
    }

    public static string GetMergeBaseSha(string exclude, string baseSha, string headSha)
    {
        return Exec([
            "git",
            "log",
            "--pretty=oneline",
            "--graph",
            exclude,
            baseSha,
            headSha
        ]).Result;
    }

    public static string GetMergeBaseSha(string baseSha, string headSha)
    {
        return Exec(["git", "merge-base", baseSha, headSha]).Result;
    }

    public static uint GetAmountOfParents(string sha)
    {
        string output = Exec(["git", "log", "--parents", "-n", "1", sha], RepoWd).Result;
        if (output.Length == 0) return 0;

        List<string> parts = output.Split(' ').ToList();

        return (uint)parts.Count - 1;
    }
}