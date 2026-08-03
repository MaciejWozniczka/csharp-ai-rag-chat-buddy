# Secrets Checked into Repository Exposed in Build Logs

## Service
CI/CD System

## Summary
A developer accidentally committed an API key, which was then printed in build logs during a test run.
The repository is private, but multiple internal teams had access.

## Root Cause
Pre-commit hooks to detect secrets were optional and not enforced.
Build steps echoed environment variables during debugging and the pattern was never removed.

## Mitigation
The exposed key was rotated immediately, enforced secret scanning was enabled for all repositories,
and build scripts were updated to avoid printing sensitive values. Training was provided on secret-handling best practices.

## Source
https://example.com/incidents/secrets-in-repo
