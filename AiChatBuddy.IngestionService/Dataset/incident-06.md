# Incorrect Feature Rollout Due to Misconfigured Targeting

## Service
Feature Flag Service

## Summary
A new experimental UI was unintentionally rolled out to all users instead of the intended 5% cohort.
Support tickets increased as some legacy browsers were not compatible with the new UI components.

## Root Cause
The targeting rule in the feature flag system reused an existing segment that included all authenticated users.
The change was not peer-reviewed, and there was no dry-run validation to show the resulting audience size.

## Mitigation
The rule was reverted, a dedicated test segment was created for experiments, and the flag system was
updated to display an estimated audience size before rollout. Peer review was made mandatory for all production rules.

## Source
https://example.com/incidents/feature-rollout-targeting
