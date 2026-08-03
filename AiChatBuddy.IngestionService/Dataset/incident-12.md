# Thundering Herd on Service Startup

## Service
User Profile Service

## Summary
After a maintenance window, all instances of the user profile service restarted simultaneously.
This caused a surge in downstream cache warmup requests and overloaded the database.

## Root Cause
A deployment script included a bulk restart command without staggering.
The service reads most data from cache on startup and immediately begins warmup queries against the database.

## Mitigation
Rolling restarts were implemented with jitter between instance restarts.
Startup warmup was optimized to use batched queries and backoff. Runbooks were updated to prohibit bulk restarts in production.

## Source
https://example.com/incidents/thundering-herd-startup
