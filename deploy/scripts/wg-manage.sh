#!/usr/bin/env bash
set -euo pipefail

COMMAND=${1:-}
USER_ID=${2:-}
ARG=${3:-}
IP_ADDRESS=${4:-}
DEVICE_ID=${5:-}

WG_INTERFACE=${WG_INTERFACE:-wg0}
WG_CONFIG=${WG_CONFIG:-/etc/wireguard/${WG_INTERFACE}.conf}
WG_ENDPOINT=${WG_ENDPOINT:-vpn.example.com:51820}
WG_DNS=${WG_DNS:-1.1.1.1}
WG_ALLOWED_IPS=${WG_ALLOWED_IPS:-0.0.0.0/0,::/0}
WG_SERVER_PUBLIC_KEY=${WG_SERVER_PUBLIC_KEY:-}
WG_MTU=${WG_MTU:-1420}
LOCK_FILE="${WG_CONFIG}.lock"

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || {
    echo "Missing dependency: $1" >&2
    exit 1
  }
}

require_cmd wg
require_cmd wg-quick
require_cmd jq

if [[ -z "$COMMAND" || -z "$USER_ID" || -z "$ARG" || -z "$IP_ADDRESS" || -z "$DEVICE_ID" ]]; then
  echo "Usage: $0 <add|remove> <user-id> <device-name|public-key> <ip-address> <device-id>" >&2
  exit 1
fi

if [[ ! -f "$WG_CONFIG" ]]; then
  echo "WireGuard config not found at $WG_CONFIG" >&2
  exit 1
fi

acquire_lock() {
  exec 9>"$LOCK_FILE"
  flock -n 9
}

sync_wireguard() {
  local stripped
  stripped=$(wg-quick strip "$WG_INTERFACE")
  wg syncconf "$WG_INTERFACE" <(echo "$stripped")
}

append_peer() {
  local device_name="$1"
  local ip="$2"
  local private_key
  private_key=$(wg genkey)
  local public_key
  public_key=$(echo "$private_key" | wg pubkey)

  cat <<EOM >> "$WG_CONFIG"

# FrenzyNet user ${USER_ID} device ${DEVICE_ID} ${device_name}
[Peer]
PublicKey = ${public_key}
AllowedIPs = ${ip}/32
EOM

  sync_wireguard

  local client_config
  client_config=$(cat <<EOM
[Interface]
PrivateKey = ${private_key}
Address = ${ip}/32
DNS = ${WG_DNS}
MTU = ${WG_MTU}

[Peer]
PublicKey = ${WG_SERVER_PUBLIC_KEY}
Endpoint = ${WG_ENDPOINT}
AllowedIPs = ${WG_ALLOWED_IPS}
PersistentKeepalive = 25
EOM
)

  printf '{"config":%s,"publicKey":%s}' "$(jq -Rs . <<<"$client_config")" "$(jq -Rs . <<<"$public_key")"
}

remove_peer() {
  local public_key="$1"

  wg set "$WG_INTERFACE" peer "$public_key" remove || true

  awk -v key="$public_key" '
    $0 ~ /^\[Peer\]/ {in_peer=1; buffer=$0; next}
    in_peer {buffer = buffer "\n" $0; if ($0 ~ /^AllowedIPs/) {if (buffer ~ key) {buffer=""} else {print buffer} in_peer=0; next}};
    {if (!in_peer) print}
  ' "$WG_CONFIG" > "${WG_CONFIG}.tmp"

  mv "${WG_CONFIG}.tmp" "$WG_CONFIG"
  sync_wireguard
  echo '{}'
}

acquire_lock

case "$COMMAND" in
  add)
    append_peer "$ARG" "$IP_ADDRESS"
    ;;
  remove)
    remove_peer "$ARG"
    ;;
  *)
    echo "Unknown command: $COMMAND" >&2
    exit 1
    ;;
esac
