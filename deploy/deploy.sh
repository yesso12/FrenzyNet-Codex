#!/usr/bin/env bash
set -euo pipefail

REPO_DIR=$(cd "$(dirname "$0")/.." && pwd)

cd "$REPO_DIR"

if [[ ! -f .env ]]; then
  echo ".env not found in $REPO_DIR" >&2
  exit 1
fi

if grep -nE '\$\(|`' .env >/dev/null; then
  echo ".env contains command substitutions. Remove them before deploying." >&2
  exit 1
fi

set -a
source .env
set +a

git pull

docker compose -f deploy/docker-compose.yml --env-file .env up -d --build

COMPOSE_FILE=deploy/docker-compose.yml ./deploy/scripts/bootstrap-db.sh

API_HEALTH=$(docker compose -f deploy/docker-compose.yml exec -T api curl -sf http://localhost:8080/health || true)
WEB_HEALTH=$(docker compose -f deploy/docker-compose.yml exec -T nginx curl -sf http://localhost || true)

cat <<SUMMARY

Deployment complete.

Health:
- API: ${API_HEALTH:-unhealthy}
- Web: ${WEB_HEALTH:-unhealthy}

URLs:
- Web: https://${DOMAIN}
- API: https://${DOMAIN}/api

SUMMARY
