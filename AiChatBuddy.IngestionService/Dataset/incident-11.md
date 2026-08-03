# Configuration Drift Causing Inconsistent Behavior Across Regions

## Service
Config Service

## Summary
Different regions returned inconsistent feature behavior and API responses.
Some customers reported missing features while others saw the new experience.

## Root Cause
Configuration files were applied manually to each region, and one region missed the latest update.
There was no single source of truth or automated configuration rollout mechanism.

## Mitigation
Configuration management was moved to a centralized git-backed system, rollouts are now automated
and tracked, and drift detection alerts if regions diverge from the desired state.

## Source
https://example.com/incidents/config-drift
