# BubblesBot GH Fast-Forward _(Fast-Forward)_
A GitHub Action that allows fast-forwarding of your branches.

Currently, GitHub's UI only offers "Create a merge commit," "Squash and merge," and "Rebase and merge". The first two 
options create a non-linear history. The third option however, comes with the caveat that the rebased commits are 
rewritten and thus, their SHAs get rehashed. This breaks any requirement a repository may have regarding commits needing
to be signed.

Git itself does offer fast-forwarding, but it's far less convenient for many developers to have to rely on the CLI to
achieve this every time they want to perform a fast-forward. This is where BubblesBot GH's Fast-Forward functionality 
comes in!

## Table of Contents

- [Usage](#usage)
- [Parameters](#parameters)
- [Contributing](#contributing)

## Usage
Make sure to install the [BubblesBot GH GitHub App](https://github.com/apps/bubblesbot-gh) (required).

See [example](example-workflow.yml) for a complete workflow using this repository's action.

**Basic Setup:**
```
jobs:
  fast-forward:
    if: github.event.comment.user.login != 'bubblesbot-gh'
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v7.0.1
        with:
          fetch-depth: 0
          
      - name: Fast-Forward
      uses: bubbles-bot-gh/fast-forward@v1
      env:
        GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        auto_merge: true
        post_comment: always
        custom_command: /fast-forward
```

> [!CAUTION]
> It is highly advised to either include the if statement, `github.event.comment.user.login != 'bubblesbot-gh'` or
  not utilize issue_comment events for this workflow. This is to prevent the bot from recursively triggering workflows.

> [!NOTE]
> BubblesBot GH currently only supports the following workflow triggers:
> - pull_request opened
> - issue_comment created
> - issue_comment edited

> [!IMPORTANT]
> The following permissions are REQUIRED:
> - id-token: write
> - contents: write
> - pull-requests: write
> - issues: write

## Parameters
- auto_merge:
  - required: false
  - default: false
  - description: Boolean that determines if the target branch should automatically be fast-forwarded upon determining 
      that a pull request is able to do so.
> [!NOTE]
> Fast-forwards can only ever be performed if the pull request/comment author is a collaborator or higher on the
  repository. If the author does NOT have collaborator or higher permissions on the repository, the fast-forward 
  will not be performed, even if auto merge is enabled.

- post_comment:
  - required: false
  - default: always
  - description: String that determines if a comment containing the status of the fast-forward will be posted on the 
      relevant pull request.
  - values:
    - "always" - Always posts a comment to the pull request indicating if it is possible to fast-forward the target branch
        and, if 'auto_merge' is true, if fast-forwarding succeeded.

    - "on-error" - A comment is only posted if an error occurs.

    - "never" - No comment is posted. The comment is still available via the GitHub Workflow's output variable and in
        the step's summary.

- custom_command:
  - required: false
  - default: /fast-forward
  - description: String that contains the command to be used to trigger BubblesBot.
> [!NOTE]
> Either a comment on the pull request or the description body of the pull request can utilize this command.
  The comment must match this command verbatim and contain no other characters (whitespace is automatically trimmed).

## Contributing

### Contributors
<a href="https://github.com/bubbles-bot-gh/fast-forward/graphs/contributors"><a href="https://github.com/bubbles-bot-gh/fast-forward/graphs/contributors">
    <img src="https://contrib.rocks/image?repo=bubbles-bot-gh/fast-forward" />
</a>