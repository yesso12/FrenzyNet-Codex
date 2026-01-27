#!/usr/bin/env bash
set -e

WG_CONF="/etc/wireguard/wg0.conf"

CMD="$1"
PUBKEY="$2"
IP="$3"

if [[ -z "$CMD" ]]; then
  echo "Usage: wg-manage.sh add|remove <public_key> <ip>"
  exit 1
fi

if [[ ! -f "$WG_CONF" ]]; then
  echo "WireGuard config not found at $WG_CONF"
  exit 1
fi

case "$CMD" in
  add)
    if grep -q "$PUBKEY" "$WG_CONF"; then
      echo "Peer already exists"
      exit 0
    fi

    cat <<EOF >> "$WG_CONF"

[Peer]
PublicKey = $PUBKEY
AllowedIPs = $IP/32
EOF

    wg syncconf wg0 <(wg-quick strip wg0)
    echo "Peer added"
    ;;
  remove)
    sed -i "/$PUBKEY/,+3d" "$WG_CONF"
    wg syncconf wg0 <(wg-quick strip wg0)
    echo "Peer removed"
    ;;
  *)
    echo "Unknown command: $CMD"
    exit 1
    ;;
esac
