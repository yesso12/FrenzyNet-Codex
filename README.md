Copyright (c) 2026 Dustin J. Trimble

All rights reserved.

This software and associated source code are proprietary and confidential.
Unauthorized copying, modification, distribution, or use of this
software, in whole or in part, is strictly prohibited without
express written permission from the copyright holder.

# Contributing

By contributing to this repository, you agree that all contributions,
including code, documentation, and related materials, become the
exclusive property of the project owner.

You grant the project owner full rights to use, modify, sublicense,
and commercialize your contributions without restriction.

Do not submit contributions unless you agree to these terms.

# FrenzyNet Gaming VPN

This project is a gaming-focused VPN platform.

Users log in via a website (email or username),
then download WireGuard VPN configs.

## Stack
- Debian 12
- WireGuard (host)
- ASP.NET 8 Web API
- React + Tailwind
- PostgreSQL
- Docker Compose
- Nginx
- Cloudflare (HTTPS only)

## Requirements
- WireGuard runs on the VPS host
- Website/API run in Docker
- Cloudflare must NOT proxy WireGuard UDP

## Core Features
- Register / Login
- User dashboard
- Generate / revoke VPN devices
- Download WireGuard config
- QR code support
- Admin controls
