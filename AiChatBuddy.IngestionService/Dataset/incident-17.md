# Misrouted Traffic After Load Balancer Rule Change

## Service
Edge Load Balancer

## Summary
A subset of traffic for a multi-tenant application was routed to the wrong backend pool.
Some tenants saw other tenants’ login pages, although no data access occurred beyond the login screen.

## Root Cause
A new routing rule used a broad hostname match that overlapped with existing rules.
The evaluation order changed when the configuration was reloaded, causing requests to hit the wrong rule.

## Mitigation
Routing rules were refactored to use explicit host and path matches.
A configuration linter was added to detect overlapping rules, and a staging environment now validates routing behavior before rollout.

## Source
https://example.com/incidents/misrouted-traffic
