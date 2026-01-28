You are to generate the full project.

Create:
- /server → ASP.NET 8 Web API
- /web → React + Tailwind frontend
- /deploy → docker-compose, nginx, scripts

Backend requirements:
- JWT authentication
- Login via email OR username
- Role-based access (user/admin)
- WireGuard peer provisioning via host commands
- Safe config updates
- Audit logging

Frontend requirements:
- Login / Register pages
- Dashboard with buttons
- Download WireGuard config
- QR code display
- Clean modern UI

Deployment:
- Docker Compose
- PostgreSQL
- Nginx reverse proxy
- Environment variable driven

Security:
- Hash passwords
- Do NOT permanently store private keys
- Confirmation dialogs for destructive actions

Billing & Limits

- Implement per-user device limits (default 3)
- Store device count per user
- Block provisioning if limit exceeded
- Admin can override limits
- Add config for future billing tiers
- UI must clearly show device usage
- Do NOT integrate Stripe yet; only scaffold billing hooks

🚨 LEGAL / SAFETY NOTE (REAL TALK)

Before selling:

Add Terms of Service
Add Acceptable Use Policy
Explicitly block:
- torrents
- scraping
- resale
- abuse
