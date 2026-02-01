# Deployment

## Environment

Copy `.env.example` to `.env` and set values, or generate one:

```bash
./deploy/gen-env.sh
# or from repo root
./gen-env.sh
```

Required values:

- `DOMAIN`
- `JWT_SECRET`
- `POSTGRES_PASSWORD`
- `WG_ENDPOINT`
- `WG_SERVER_PUBLIC_KEY`
- `WG_ADDRESS_POOL`
- `WG_ALLOWED_IPS`
- `WG_DNS`
- `WG_MTU`
- `ADMIN_EMAIL`
- `ADMIN_PASSWORD`
- `OWNER_EMAIL`
- `OWNER_PASSWORD`
- `OWNER_BOOTSTRAP`

## One-command deploy

```bash
./deploy/deploy.sh
```

The API is served under `https://frenzynets.com/api`.

## Fresh VPS install

```bash
sudo ./deploy/install.sh
```

## Database schema

The deploy script automatically applies `deploy/schema.sql`.
You can re-apply manually if needed:

```bash
./deploy/scripts/bootstrap-db.sh
```

## WireGuard integration

The API container mounts `/etc/wireguard` from the host and calls
`/usr/local/bin/wg-manage.sh` to add/revoke peers. Ensure the host has:

- `wg` and `wg-quick`
- `jq` for JSON escaping
- a `wg0.conf` configured

`wg-manage.sh` appends new peers to `wg0.conf` and avoids storing private keys.

## Owner bootstrap

To create the initial owner account, set these in `.env`:

```
OWNER_BOOTSTRAP=true
OWNER_EMAIL=trimbledustn@gmail.com
OWNER_PASSWORD=your-strong-password
```

After the first successful run, set `OWNER_BOOTSTRAP=false`.

## Cloudflare DNS records

- `frenzynets.com` → proxied (orange cloud)
- `api.frenzynets.com` → proxied (orange cloud) if you use the API subdomain
- `vpn.frenzynets.com` → DNS-only (gray cloud) for WireGuard UDP

## Production checklist

- [ ] Cloudflare origin certs mounted at `/etc/ssl/cloudflare/origin.pem` and `/etc/ssl/cloudflare/origin.key`
- [ ] `JWT_SECRET` set to a long random string
- [ ] `POSTGRES_PASSWORD` rotated and stored securely
- [ ] `WG_SERVER_PUBLIC_KEY` set from `wg show wg0 public-key`
- [ ] `WG_ADDRESS_POOL` configured and not overlapping existing subnets
- [ ] `OWNER_BOOTSTRAP` run once and disabled
- [ ] `wg0.conf` secured and backed up
- [ ] WireGuard UDP port (51820) open to the internet
