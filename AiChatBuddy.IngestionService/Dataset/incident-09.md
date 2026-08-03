# Rate Limiter Misconfiguration Blocking Internal Services

## Service
API Gateway

## Summary
Internal services started receiving 429 responses from the API gateway during a production load test.
External customers were mostly unaffected, but background jobs and internal tools failed.

## Root Cause
A global rate limiting rule did not distinguish between internal and external traffic.
The test traffic came from internal IP ranges and consumed the shared rate limit, starving background processing jobs.

## Mitigation
Separate rate limit buckets were created for internal and external traffic.
The configuration was updated to use service identity rather than IP ranges, and load test IPs were excluded from global limits.

## Source
https://example.com/incidents/rate-limiter-internal
