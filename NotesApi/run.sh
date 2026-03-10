#!/bin/bash
# Run the Notes API (use this if dotnet is not in your PATH)
set -e
cd "$(dirname "$0")"
DOTNET="${DOTNET:-dotnet}"
if ! command -v dotnet &>/dev/null; then
  DOTNET="/opt/homebrew/Cellar/dotnet@8/8.0.124/bin/dotnet"
fi
"$DOTNET" run
