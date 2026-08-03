# Database Deadlocks After Schema Change

## Service
Billing Service

## Summary
Invoice generation started failing for a subset of customers and timed out for others.
Support observed repeated retries in the billing UI and a growing backlog of unprocessed invoices.

## Root Cause
A schema migration introduced a new index on a frequently updated column.
The resulting lock order changed and led to an increase in deadlocks between the invoice writer and a reporting reader.
The ORM retry policy retried too aggressively, amplifying the load.

## Mitigation
The new index was removed, and the migration was redesigned and tested in a staging environment with
production-like traffic. Deadlock metrics were added, and the ORM retry policy was tuned to use jitter and backoff.

## Source
https://example.com/incidents/billing-deadlocks
