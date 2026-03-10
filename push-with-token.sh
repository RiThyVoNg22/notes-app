#!/bin/bash
# One-time script to push the workflow file using a Personal Access Token.
# GitHub blocks OAuth apps from updating workflows; a PAT with "workflow" scope works.
#
# 1. Create a token: https://github.com/settings/tokens/new
#    - Scopes: check "repo" and "workflow"
#    - Generate, then copy the token
# 2. Run this script and paste your token when asked for password:
#    chmod +x push-with-token.sh
#    ./push-with-token.sh

set -e
cd "$(dirname "$0")"

echo "You need a Personal Access Token with 'workflow' scope."
echo "Create one at: https://github.com/settings/tokens/new (check 'repo' and 'workflow')"
echo ""
echo "When 'git push' asks for password, paste the token (not your GitHub password)."
echo ""

git push origin main
