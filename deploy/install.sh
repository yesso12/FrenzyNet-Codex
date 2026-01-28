#!/usr/bin/env bash
set -euo pipefail

REPO_DIR=/opt/FrenzyNet-Codex
REPO_URL=${REPO_URL:-git@github.com:yesso12/FrenzyNet-Codex.git}

if ! command -v docker >/dev/null 2>&1; then
  echo "Docker not installed. Install Docker before running this script." >&2
  exit 1
fi

if [[ ! -d "$REPO_DIR" ]]; then
  git clone "$REPO_URL" "$REPO_DIR"
fi

cd "$REPO_DIR"

if [[ ! -f .env ]]; then
  echo ".env not found. Copy .env.example to .env and update values." >&2
  exit 1
fi

./deploy/deploy.sh
