# Authentication Outage After Certificate Rotation

## Service
Identity Platform

## Summary
Users were unable to sign in for 27 minutes after a routine certificate rotation.
Token validation started failing for all newly issued tokens across web and mobile clients.

## Root Cause
The signing certificate for JSON Web Tokens was rotated, but a legacy validation
service still depended on a statically configured public key. The configuration management pipeline
did not update this dependency and health checks did not cover the legacy path.

## Mitigation
The old certificate was temporarily restored, and the legacy validation service was
migrated to use the shared key-discovery endpoint. Additional health checks were added to validate
token issuance and validation end-to-end after future rotations.

## Source
https://example.com/incidents/auth-cert-rotation
