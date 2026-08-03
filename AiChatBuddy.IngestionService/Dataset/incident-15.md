# DNS Misconfiguration Leading to Partial Outage

## Service
Public API

## Summary
Some customers were unable to resolve the public API hostname for roughly 40 minutes.
The impact varied by ISP and region.

## Root Cause
A DNS record change was applied with an incorrect CNAME target.
Due to caching and TTL behavior, some resolvers continued to serve the old value while others switched to the invalid target.

## Mitigation
The DNS record was corrected and TTL values were temporarily reduced.
A formal change-review process was introduced for DNS updates, and a staging zone is used to validate changes before production.

## Source
https://example.com/incidents/dns-misconfig
