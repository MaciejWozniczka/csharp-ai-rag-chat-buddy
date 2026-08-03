# Slow Admin Dashboard Due to N+1 Queries

## Service
Admin Portal

## Summary
Admins experienced very slow page loads on a dashboard that lists customer accounts.
The slowdown was especially noticeable for tenants with many linked resources.

## Root Cause
A new column was added to the dashboard that triggered an additional database query per row
to fetch related statistics. This resulted in N+1 query behavior under the hood.

## Mitigation
The query was rewritten to prefetch related data in a single join and to paginate results.
Performance tests were added for common admin views to detect similar regressions early.

## Source
https://example.com/incidents/admin-n-plus-one
