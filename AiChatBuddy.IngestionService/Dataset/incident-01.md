# API Latency Spike in Orders Service

## Service
Orders API

## Summary
Customers reported very slow checkout times in the orders service for about 45 minutes.
Requests completed successfully but p95 latency jumped from ~300ms to more than 8 seconds.

## Root Cause
A feature flag enabled synchronous calls to an external tax-calculation API
on the hot path. The external API rate-limited traffic and the client implementation lacked timeouts,
causing threads to block and the thread pool to exhaust.

## Mitigation
The feature flag was rolled back, client timeouts were added to the tax API,
and an asynchronous background calculation path was implemented. Traffic to the tax provider is now
limited and cached results are reused whenever possible.

## Source
https://example.com/incidents/orders-api-latency
