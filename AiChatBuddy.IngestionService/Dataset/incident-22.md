# Misconfigured CORS Blocking Browser Clients

## Service
REST API

## Summary
Web clients suddenly started receiving CORS errors in the browser console and could not
call certain API endpoints, while server-to-server integrations worked fine.

## Root Cause
A security hardening change modified the allowed origins list but omitted an important wildcard rule.
The change passed automated tests because they used a localhost origin that remained allowed.

## Mitigation
The missing origin pattern was restored, and a more explicit list of allowed domains was defined.
End-to-end tests were updated to run from the same origins as production web apps.

## Source
https://example.com/incidents/cors-misconfig
