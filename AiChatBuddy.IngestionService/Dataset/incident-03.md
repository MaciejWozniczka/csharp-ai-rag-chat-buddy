# Cache Stampede on Product Detail Endpoint

## Service
Catalog Service

## Summary
Product detail pages intermittently returned 500 errors and timeouts during a large sale event.
Downstream databases experienced a sudden spike in read traffic.

## Root Cause
A global cache eviction was triggered after a configuration change. The cache layer did not
use request coalescing, so thousands of concurrent requests all missed cache and went directly to the database.
The database connection pool saturated and started rejecting new connections.

## Mitigation
Request coalescing was implemented for cache-miss scenarios, a staggered cache warm-up job was
introduced, and safeguards were added to prevent full-cache eviction during peak periods.

## Source
https://example.com/incidents/catalog-cache-stampede
