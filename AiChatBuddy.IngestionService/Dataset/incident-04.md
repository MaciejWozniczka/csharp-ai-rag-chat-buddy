# Background Job Queue Backlog Causing Delayed Emails

## Service
Notifications Service

## Summary
Email notifications were delayed by up to 3 hours for roughly 18% of users.
Other notification channels such as push and SMS were unaffected.

## Root Cause
A new reporting job was deployed that consumed messages from the same queue as the email worker.
The reporting job had an expensive per-message processing step and was misconfigured to use a high concurrency level,
competing for the same messages and slowing down email delivery.

## Mitigation
The reporting job was temporarily disabled, then moved to a dedicated queue.
Queue throughput and per-consumer metrics were added to dashboards, and SLO alerts were defined for maximum email delay.

## Source
https://example.com/incidents/delayed-emails
