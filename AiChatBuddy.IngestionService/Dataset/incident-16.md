# Logging Pipeline Outage Hiding Real-Time Errors

## Service
Central Logging

## Summary
During an unrelated incident, teams could not see new log entries in the central logging system.
This slowed diagnosis and increased time-to-resolution.

## Root Cause
A storage quota was reached in the logging backend, causing ingestion to backpressure and eventually drop data.
Alerts for the logging system were informational only and not wired into on-call rotations.

## Mitigation
Storage was expanded, retention policies were adjusted, and critical platforms such as logging and metrics
were explicitly added to incident response runbooks and paging policies.

## Source
https://example.com/incidents/logging-outage
