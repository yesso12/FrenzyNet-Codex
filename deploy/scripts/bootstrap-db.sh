#!/usr/bin/env bash
set -euo pipefail

COMPOSE_FILE=${COMPOSE_FILE:-./docker-compose.yml}
SCHEMA_FILE=${SCHEMA_FILE:-$(dirname "$0")/../schema.sql}
DB_USER=${DB_USER:-postgres}
DB_NAME=${DB_NAME:-frenzynet}

if [[ ! -f "$SCHEMA_FILE" ]]; then
  echo "Schema file not found: $SCHEMA_FILE" >&2
  exit 1
fi

docker compose -f "$COMPOSE_FILE" exec -T postgres \
  psql -U "$DB_USER" -d "$DB_NAME" -f /schema.sql \
  || {
    echo "Failed to apply schema." >&2
    exit 1
  }
