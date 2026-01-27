#!/usr/bin/env bash
set -euo pipefail

echo "[+] FrenzyNet deploy starting..."

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

if [ ! -f ".env" ]; then
  echo "[!] .env file missing in $ROOT_DIR"
  exit 1
fi

echo "[+] Pulling latest code..."
git pull

echo "[+] Building and starting containers..."
cd deploy
docker-compose --env-file ../.env up -d --build

echo "[+] Waiting for database..."
sleep 5

echo "[+] Deploy complete."
docker-compose ps
