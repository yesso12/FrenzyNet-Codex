#!/usr/bin/env bash
set -euo pipefail

REPO_DIR=/opt/FrenzyNet-Codex

if [[ ! -d "$REPO_DIR" ]]; then
  echo "Repository not found at $REPO_DIR" >&2
  exit 1
fi

cd "$REPO_DIR"

if [[ -f .env ]]; then
  set -a
  source .env
  set +a
else
  echo ".env not found in $REPO_DIR" >&2
  exit 1
fi

git pull

docker compose -f deploy/docker-compose.yml --env-file .env up -d --build

COMPOSE_FILE=deploy/docker-compose.yml ./deploy/scripts/bootstrap-db.sh

API_HEALTH=$(docker compose -f deploy/docker-compose.yml exec -T api curl -sf http://localhost:8080/health || true)

cat <<SUMMARY

Deployment complete.

Health:
- API: ${API_HEALTH:-unhealthy}

URLs:
- Web: https://${DOMAIN}
- API: https://${DOMAIN}/api

SUMMARY
