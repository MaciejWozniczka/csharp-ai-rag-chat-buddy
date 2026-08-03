# Overloaded Message Broker During Peak Traffic

## Service
Event Bus

## Summary
During a promotional campaign, message throughput increased by 5x and the central broker became saturated.
Consumers fell behind, and some time-sensitive events were processed too late to be useful.

## Root Cause
Capacity planning for the broker assumed linear growth and did not account for promotional spikes.
Partitions were unevenly distributed, and a noisy neighbor tenant produced a disproportionate amount of events.

## Mitigation
The broker cluster was scaled out, partitioning was rebalanced, and per-tenant quotas were introduced.
Load testing scenarios were updated to include promotional spikes and burst scenarios.

## Source
https://example.com/incidents/broker-overload
