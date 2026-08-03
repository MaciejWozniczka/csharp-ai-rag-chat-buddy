# Stuck Batch Job Due to Clock Skew

## Service
Reporting Service

## Summary
Daily reporting jobs stopped producing new reports even though input data continued to arrive.
Operators noticed that the scheduler logs showed jobs as 'running' indefinitely.

## Root Cause
One node in the scheduler cluster had significant clock skew and repeatedly acquired the leadership lock.
It then scheduled jobs in the past, which were ignored by workers that relied on correct timestamps.

## Mitigation
Time synchronization settings were fixed across the cluster, and monitoring was added for clock drift.
The scheduler was updated to use monotonic time for internal coordination.

## Source
https://example.com/incidents/clock-skew
