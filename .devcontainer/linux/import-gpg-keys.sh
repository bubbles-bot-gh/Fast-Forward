#!/bin/bash

KEY_FILE="/workspaces/${1}/tmp/.gpg-export.gpg"
META_FILE="/workspaces/${1}/tmp/.gpg-export.meta"
echo "Importing GPG keys from $KEY_FILE"

if [ -f "$KEY_FILE" ]; then
    if gpg --import "$KEY_FILE"; then
        echo "GPG keys imported. Removing temporary file: $KEY_FILE"
    else
        echo "Warning: gpg import failed for $KEY_FILE">&2
        exit 1
    fi

    # Determine signing key
    if [ -f "$META_FILE" ]; then
      GIT_EMAIL="$(git config --global user.email 2>/dev/null || true)"
      SIGNING_KEY=""

      if [[ -n "$GIT_EMAIL" ]]; then
        SIGNING_KEY="$(awk -F'\t' -v email="$GIT_EMAIL" \
          'index($2, "<" email ">")" { print $1; exit }' "$META_FILE")"
      fi

      # Fallback: only one key imported. Use that
      if [[ -z "$SIGNING_KEY" && $(wc -l < "$META_FILE") -eq 1 ]]; then
        SIGNING_KEY="$(cut -f1 "$META_FILE" | head -n1)"
      fi

      if [[ -n "$SIGNING_KEY" ]]; then
        git config --global user.signingkey "$SIGNING_KEY"
        git config --global commit.gpgsign true
        git config --global tag.gpgsign true
        git config --global gpg.program gpg
        echo "Configured git to sign commits with key: $SIGNING_KEY"
      else
        echo "Warning: could not determine a unique signing key automatically."
        echo "Available keys:" >&2
        cat "$META_FILE" >&2
        echo "Set one manually with: git config --global user.signingkey <FPR> or through your IDE's settings"
      fi

      rm -f "$META_FILE"
    fi

    rm -f "$KEY_FILE"
    rmdir "/workspaces/${1}/tmp" 2>/dev/null || true
fi
