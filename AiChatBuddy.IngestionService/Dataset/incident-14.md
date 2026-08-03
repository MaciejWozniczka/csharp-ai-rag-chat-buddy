# Memory Leak in Background Worker Causing Node Restarts

## Service
Analytics Worker

## Summary
Analytics jobs slowed down over several hours and some worker nodes restarted unexpectedly.
Monitoring showed steadily increasing memory usage and eventual OOM kills by the orchestrator.

## Root Cause
A new job type loaded large result sets into memory but did not release references after processing.
The worker reused a long-lived in-memory cache for temporary data and never cleared it for that path.

## Mitigation
The job was updated to stream results instead of loading everything into memory.
Memory profiles were added to CI performance tests, and alerts were created for abnormal heap growth in workers.

## Source
https://example.com/incidents/analytics-memory-leak
