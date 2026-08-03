# Canary Deployment Skipped Due to Automation Bug

## Service
Deployment Pipeline

## Summary
A faulty build was rolled out directly to 100% of production servers instead of passing through
the canary stage. The error caused intermittent failures in one critical API for 11 minutes.

## Root Cause
A recent change in the deployment pipeline introduced a conditional step that evaluated to false
for all canary environments, effectively skipping them. The change was not covered by pipeline unit tests.

## Mitigation
The pipeline change was reverted, tests were added to ensure canary stages run for all deployments,
and a manual approval gate was introduced for production rollouts until confidence in the pipeline is re-established.

## Source
https://example.com/incidents/canary-skip
