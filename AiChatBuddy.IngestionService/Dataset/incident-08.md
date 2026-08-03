# Search Relevance Degradation After Index Rebuild

## Service
Search Service

## Summary
Users reported poor search results and missing documents for several hours.
Search queries returned fewer matches and relevance rankings changed unexpectedly.

## Root Cause
An index rebuild job used an outdated configuration that excluded several document types
and changed the scoring profile. The configuration file was maintained in a separate repository and was not updated
when the search pipeline evolved.

## Mitigation
The previous index snapshot was restored, configs were moved into the main repository,
and automated integration tests now validate that sample queries continue to return expected results before deploying.

## Source
https://example.com/incidents/search-index-rebuild
