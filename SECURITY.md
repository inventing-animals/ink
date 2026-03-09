# Security Policy

## Reporting a Vulnerability

Please do not report security vulnerabilities through public GitHub issues.

Instead, email us at **security@inventing-animals.com**. Include as much detail as you can: what the issue is, how to reproduce it, and what the potential impact might be. We will respond as quickly as possible and keep you informed as we work on a fix.

## Scope

As a UI component library, the attack surface is narrow but real. The following are considered security issues:

- Any functionality that collects, transmits, or exposes user data
- Telemetry, analytics, or any form of network call not explicitly initiated by the consuming application
- Debugging hooks, backdoors, or diagnostic features left in non-debug builds
- Dependencies with known vulnerabilities

## Policy

Ink must be safe to embed in any application without introducing trust concerns of its own. Contributions that add data collection, remote calls, or hidden diagnostic functionality will be rejected unconditionally, regardless of intent.

If you are unsure whether something crosses this line, ask before opening a pull request.
