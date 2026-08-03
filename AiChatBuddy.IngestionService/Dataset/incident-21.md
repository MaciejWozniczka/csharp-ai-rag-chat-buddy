# Autoscaling Policy Flapping Under Moderate Load

## Service
Recommendation Service

## Summary
The recommendation service scaled up and down repeatedly over a short period,
causing cold starts and degraded performance during each scale event.

## Root Cause
The autoscaling policy used CPU utilization with very narrow thresholds and no stabilization window.
Minor fluctuations triggered scale-out and scale-in events back-to-back.

## Mitigation
Stabilization windows and minimum scale durations were introduced.
Autoscaling decisions now consider multiple metrics, including request rate and error percentage.

## Source
https://example.com/incidents/autoscaling-flap
