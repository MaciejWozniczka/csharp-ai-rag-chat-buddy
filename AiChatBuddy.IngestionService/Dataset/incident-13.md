# Incorrect Rollback Restored Old Buggy Version

## Service
Payments Service

## Summary
Attempted rollback after a failed deployment accidentally restored a much older version of the service.
This reintroduced a previously fixed bug that caused duplicate payment attempts in some flows.

## Root Cause
The rollback script referenced a floating 'previous' tag instead of an explicit version.
The tag was not consistently updated, so operators chose an incorrect artifact when executing the rollback.

## Mitigation
Rollback procedures were changed to use explicit version numbers and verified artifacts.
Deployment tooling now shows a preview of the target version for both forward and rollback operations.

## Source
https://example.com/incidents/incorrect-rollback
