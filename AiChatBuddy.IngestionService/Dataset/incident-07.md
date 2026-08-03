# File Storage Unavailability After Metadata Service Upgrade

## Service
File Storage

## Summary
Customers could not upload or download files for approximately 22 minutes in one region.
Error rates spiked and the console showed generic 500 errors.

## Root Cause
The metadata service that stores file references was upgraded to a new version requiring
a different TLS configuration. One of the internal clients was pinned to an older TLS protocol and failed to negotiate,
causing all requests from that region to fail.

## Mitigation
The deployment was rolled back, client libraries were updated to support the new TLS settings,
and a compatibility matrix was documented. A canary environment was created that mirrors regional client diversity.

## Source
https://example.com/incidents/file-storage-metadata
