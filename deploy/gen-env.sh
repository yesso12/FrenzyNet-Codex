#!/usr/bin/env bash
set -euo pipefail

ENV_FILE=${ENV_FILE:-.env}
FORCE=${1:-}

if [[ -f "$ENV_FILE" && "$FORCE" != "--force" ]]; then
  echo "$ENV_FILE already exists. Use --force to overwrite." >&2
  exit 1
fi

JWT_SECRET=$(openssl rand -hex 32)
POSTGRES_PASSWORD=$(openssl rand -hex 16)
OWNER_PASSWORD=$(openssl rand -hex 16)
ADMIN_PASSWORD=$(openssl rand -hex 16)

cat <<ENV > "$ENV_FILE"
DOMAIN=frenzynets.com
API_BASE_URL=https://frenzynets.com/api
JWT_SECRET=${JWT_SECRET}
POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
WG_ENDPOINT=vpn.frenzynets.com:51820
WG_SERVER_PUBLIC_KEY=replace-with-server-public-key
WG_ADDRESS_POOL=10.0.0.0/24
WG_ALLOWED_IPS=0.0.0.0/0,::/0
WG_DNS=1.1.1.1
WG_MTU=1420
WEB_ORIGIN=https://frenzynets.com
ADMIN_EMAIL=admin@frenzynets.com
ADMIN_PASSWORD=${ADMIN_PASSWORD}
ADMIN_USERNAME=admin
OWNER_EMAIL=trimbledustn@gmail.com
OWNER_PASSWORD=${OWNER_PASSWORD}
OWNER_BOOTSTRAP=false
ENV

chmod 600 "$ENV_FILE"

echo "Generated $ENV_FILE. Update WG_SERVER_PUBLIC_KEY and OWNER_BOOTSTRAP as needed."
