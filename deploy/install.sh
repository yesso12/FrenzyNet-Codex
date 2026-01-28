#!/usr/bin/env bash
set -euo pipefail

REPO_DIR=/opt/FrenzyNet-Codex
REPO_URL=${REPO_URL:-git@github.com:yesso12/FrenzyNet-Codex.git}
WG_INTERFACE=${WG_INTERFACE:-wg0}
WG_CONFIG=/etc/wireguard/${WG_INTERFACE}.conf

if [[ $(id -u) -ne 0 ]]; then
  echo "Run as root." >&2
  exit 1
fi

apt-get update
apt-get install -y --no-install-recommends \
  ca-certificates curl gnupg lsb-release ufw jq openssl qrencode git \
  wireguard wireguard-tools postgresql-client

if ! command -v docker >/dev/null 2>&1; then
  apt-get install -y --no-install-recommends docker.io docker-compose-plugin
  systemctl enable --now docker
fi

sysctl -w net.ipv4.ip_forward=1
if ! grep -q "net.ipv4.ip_forward=1" /etc/sysctl.conf; then
  echo "net.ipv4.ip_forward=1" >> /etc/sysctl.conf
fi

if [[ ! -f "$WG_CONFIG" ]]; then
  umask 077
  mkdir -p /etc/wireguard
  private_key=$(wg genkey)
  public_key=$(echo "$private_key" | wg pubkey)
  cat <<WGCONF > "$WG_CONFIG"
[Interface]
Address = 10.0.0.1/24
ListenPort = 51820
PrivateKey = ${private_key}

# Server public key: ${public_key}
WGCONF
  chmod 600 "$WG_CONFIG"
fi

if ! grep -q "FRENZYNET NAT" /etc/ufw/before.rules; then
  default_iface=$(ip route | awk '/default/ {print $5; exit}')
  cat <<UFW >> /etc/ufw/before.rules

# FRENZYNET NAT
*nat
:POSTROUTING ACCEPT [0:0]
-A POSTROUTING -s 10.0.0.0/24 -o ${default_iface} -j MASQUERADE
COMMIT
UFW
fi

ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp
ufw allow 51820/udp
ufw --force enable

systemctl enable --now wg-quick@${WG_INTERFACE}

if [[ ! -d "$REPO_DIR" ]]; then
  git clone "$REPO_URL" "$REPO_DIR"
fi

cd "$REPO_DIR"

if [[ ! -f .env ]]; then
  echo ".env not found. Run ./deploy/gen-env.sh and update values." >&2
  exit 1
fi

./deploy/deploy.sh
