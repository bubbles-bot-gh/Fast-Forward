using BubblesBotGitHub.FastForward.Application.Interfaces;
using Octokit;
using IGitHubClient = BubblesBotGitHub.FastForward.Infrastructure.Services.GitHubClient.IGitHubClient;

namespace BubblesBotGitHub.FastForward.Infrastructure.Services;

public class CommentBuilder(IActionInfo actionInfo, IGitHubClient gitHubClient, IGit git) : ICommentBuilder
{
    private readonly string _baseFullRef = $"{actionInfo.PrInfo.BaseRef} ({actionInfo.PrInfo.BaseSha})";
    private readonly string _headFullRef = $"{actionInfo.PrInfo.HeadRef} ({actionInfo.PrInfo.HeadSha})";
    
    public async Task<string> Build()
    {
        List<string> comment = [
            CreateVerifyingLine(),
            await AddShellBlocks()
        ];

        if (!actionInfo.EventInfo.IsPossible)
        {
            // Fast-forward is not possible
            comment.Add(await GetNotPossibleLines());
            actionInfo.EventInfo.ShouldExit = true;
        } else if (!actionInfo.ActionOptions.IsAutoMerge)
        {
            comment.Add(GetAutoMergeDisabledLine());
            actionInfo.EventInfo.ShouldExit = true;
        } else if (!await actionInfo.EventInfo.UserHasPerms)
        {
            comment.Add(GetNoPermsLine());
            actionInfo.EventInfo.ShouldExit = true;
        } else if (!actionInfo.EventInfo.CommandInvoked)
        {
            comment.Add(GetCommandNotInvokedLine());
            actionInfo.EventInfo.ShouldExit = true;
        }
        
        return string.Join('\n', comment);
    }

    private string CreateVerifyingLine()
    {
        List<string> result = [];
        result.Add(actionInfo.ActionOptions.IsAutoMerge
            ? "Auto merge enabled. Verifying and then attempting to"
            : "Auto merge disabled. Verifying we can");

        result.Add($"fast-forward {_baseFullRef} to ${_headFullRef}.");
        return string.Join(' ', result);
    }

    private async Task<string> GetNotPossibleLines()
    {
        List<string> result = [];
        
        // Determine divergence in branches
        result.Add($"Can't fast-forward {_baseFullRef} to {_headFullRef}."
            + $"{_baseFullRef} is not a direct ancestor of {_headFullRef}.");
        
        // Show where branches diverged
        // TODO: refactor IPrInfo.MergeBaseSha to allow null
        if (actionInfo.PrInfo.MergeBaseSha is null)
        {
            // No common ancestor
            result.Add("Branches do not appear to have a common ancestor.");

            return string.Join(' ', result);
        }
        
        // Divergence point was found
        result.AddRange([
            $"Branches appear to have diverged at {actionInfo.PrInfo.MergeBaseSha}.",
            "```shell"
        ]);

        string exclude = actionInfo.PrInfo.MergeBaseParentsAmount > 0
            ? actionInfo.PrInfo.MergeBaseSha
            : string.Empty;
        
        result.AddRange([
            await git.LogCommitGraph(exclude, actionInfo.PrInfo.BaseSha, actionInfo.PrInfo.HeadSha, "./tmp"),
            $"Rebase {actionInfo.PrInfo.HeadRef} onto {actionInfo.PrInfo.BaseRef} "
            + $"and then force push to {actionInfo.PrInfo.HeadRef}"
        ]);
        
        return string.Join('\n', result);
    }

    private string GetAutoMergeDisabledLine() =>
        $"It is possible to fast-forward {_baseFullRef} to {_headFullRef}, but 'auto_merge' has been disabled.";

    private string GetNoPermsLine() =>
        // Add a zero-width character to avoid pinging the user
        $"Sorry, @\u200B{actionInfo.EventInfo.User}, while it is possible to fast-forward {_headFullRef}, it appears "
        + $"you do not have permission to push to this repository. A user with the proper permission(s) can "
        + $"use the command, `{actionInfo.ActionOptions.CustomCommand}` to reattempt a fast-forward.";

    private string GetCommandNotInvokedLine() =>
        $"It is possible to fast-forward {_baseFullRef} to {_headFullRef}. If you have write access to the target "
        + $"repository, you can add the comment `{actionInfo.ActionOptions.CustomCommand}` to initiate the fast-forward.";

    private async Task<string> AddShellBlocks()
    {
        List<string> result = [];
        
        // Create target code shell block
        string baseShortRef = actionInfo.PrInfo.BaseRef;
        string baseOwner = actionInfo.RepoInfo.Owner;
        string baseName = actionInfo.RepoInfo.Name;
        string baseSha = actionInfo.PrInfo.BaseSha;
        
        result.Add($"Target Branch ({baseShortRef}):");
        result = [.. result, .. await CreateShellBlock(baseOwner, baseName, baseSha)];
        
        // Create head code shell block
        string headShortRef = actionInfo.PrInfo.HeadRef;
        string headOwner = actionInfo.PrInfo.HeadRepoOwner;
        string headName = actionInfo.PrInfo.HeadRepoName;
        string headSha = actionInfo.PrInfo.HeadSha;
        
        result.Add($"Head Branch ({headShortRef}):");
        result = result.Concat(await CreateShellBlock(headOwner, headName, headSha)).ToList();

        return string.Join('\n', result);
    }

    private async Task<List<string>> CreateShellBlock(string owner, string repoName, string sha)
    {
        GitHubCommit commit = await gitHubClient.GetCommit(owner, repoName, sha);
        string headRef = actionInfo.PrInfo.HeadRef;
        string baseRef = actionInfo.PrInfo.BaseRef;
        string authorName = commit.Commit.Author.Name;
        string authorEmail = commit.Commit.Author.Email;
        string commitDate = commit.Commit.Committer.Date.ToString("yyyy-MM-dd HH:mm:ss");
        string commitMessage = commit.Commit.Message;

        return [
            "```shell",
            $"commit {sha} (HEAD -> {headRef}, origin/{baseRef}",
            $"Author: {authorName} <{authorEmail}>",
            $"Date:   {commitDate}",
            "\n",
            $"    {commitMessage}",
            "```"
        ];
    }
}