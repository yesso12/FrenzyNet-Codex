#!/usr/bin/env bash
set -euo pipefail

DOMAIN="${DOMAIN:-frenzynets.com}"

echo "[+] Installing base deps..."
apt update
apt install -y ca-certificates curl git ufw jq wireguard wireguard-tools

echo "[+] Enabling forwarding..."
sysctl -w net.ipv4.ip_forward=1
grep -q '^net.ipv4.ip_forward=1' /etc/sysctl.conf || echo 'net.ipv4.ip_forward=1' >> /etc/sysctl.conf

echo "[+] Ensuring WireGuard keys..."
mkdir -p /etc/wireguard
if [ ! -f /etc/wireguard/server.key ]; then
  wg genkey | tee /etc/wireguard/server.key | wg pubkey > /etc/wireguard/server.pub
fi

if [ ! -f /etc/wireguard/wg0.conf ]; then
  cat > /etc/wireguard/wg0.conf <<EOF
[Interface]
Address = 10.0.0.1/24
ListenPort = 51820
PrivateKey = $(cat /etc/wireguard/server.key)
SaveConfig = true
EOF
fi

systemctl enable wg-quick@wg0
systemctl restart wg-quick@wg0 || true

echo "[+] Setting NAT..."
IFACE="$(ip route | awk '/default/ {print $5; exit}')"
iptables -t nat -C POSTROUTING -o "$IFACE" -j MASQUERADE 2>/dev/null || iptables -t nat -A POSTROUTING -o "$IFACE" -j MASQUERADE
iptables -C FORWARD -i wg0 -j ACCEPT 2>/dev/null || iptables -A FORWARD -i wg0 -j ACCEPT
iptables -C FORWARD -o wg0 -m state --state RELATED,ESTABLISHED -j ACCEPT 2>/dev/null || iptables -A FORWARD -o wg0 -m state --state RELATED,ESTABLISHED -j ACCEPT

echo "[+] Opening firewall..."
ufw allow OpenSSH
ufw allow 80/tcp
ufw allow 443/tcp
ufw allow 51820/udp
ufw --force enable

echo "[+] Done. Next: create Cloudflare origin cert files in /etc/ssl/cloudflare and run deploy."
