# Orphaned Feature Flag Causing Confusing UI Behavior

## Service
Web Frontend

## Summary
Some users saw partially rolled-out UI elements with missing buttons and inconsistent behavior.
The pattern was hard to reproduce and appeared only for specific accounts.

## Root Cause
An old feature flag that controlled a portion of the UI was never fully removed.
New features assumed the flag was always on, but for a subset of users it remained off due to legacy targeting rules.

## Mitigation
The orphaned flag was removed, and a cleanup process was created for deprecating flags.
Tests were added to ensure that new UI features do not depend on stale flags.

## Source
https://example.com/incidents/orphaned-flag
