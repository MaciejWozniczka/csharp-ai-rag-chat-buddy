# Broken Client SDK Causing Invalid Requests

## Service
Public SDK

## Summary
A new version of the public SDK generated malformed API requests.
Customers who upgraded experienced unexpected 4xx responses and failures in their integrations.

## Root Cause
A serialization change in the SDK did not handle null values correctly and produced invalid payloads.
The SDK tests relied on mocked endpoints and did not run against a real staging API.

## Mitigation
The SDK release was yanked, and a fixed version was published with additional integration tests
against a staging environment. A deprecation notice was sent to affected customers with guidance on upgrading.

## Source
https://example.com/incidents/broken-sdk
