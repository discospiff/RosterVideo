# InAppDatabase Feature — Requirements Quality Checklist

Purpose: Validate that the InAppDatabase feature specification is complete, unambiguous, and testable. This checklist focuses on requirements clarity, configuration, security, migration behavior, and deployment/run-time guarantees. Do NOT mark items as [x] here — reviewers should inspect and check items during PR review.

Configuration & Detection
- [ ] The spec clearly defines the configuration keys and their expected types/values: UseAzureDatabase (bool) and MYSQLCONNSTR_localdb (string).
- [ ] Rules for detection precedence are unambiguous: explicit UseAzureDatabase flag takes precedence over automatic Azure detection (WEBSITE_SITE_NAME fallback).
- [ ] The behavior when UseAzureDatabase is unset and WEBSITE_SITE_NAME exists is clearly specified (use DB only if connection string present).
- [ ] The normalization steps for MYSQLCONNSTR_localdb are specified (which parts to parse: host, user id, password, port, database) and an authoritative example is provided.

Connection String Normalization
- [ ] The spec references the exact expected input format from Azure In App (example value) and the expected output connection string for Pomelo/MySql.
- [ ] Handling of dynamic ports is specified (do not assume 3306) and tests or examples cover non-3306 ports.
- [ ] Error handling is defined for malformed or missing MYSQLCONNSTR_localdb: what is logged, and whether the app falls back to in-memory or fails startup.

DI & Runtime Behavior
- [ ] The DI registration strategy is fully described: default in-memory singleton; replace with DbContext+scoped DbRosterStore when DB enabled.
- [ ] Lifetime and thread-safety implications are specified (e.g., singleton vs scoped) and justified.
- [ ] The effect on existing pages (Index form/list) is explicitly stated: no user-visible change other than persistence semantics.

EF Core & Migrations
- [ ] The migration strategy is clearly defined: auto-apply migrations at startup only when DB enabled in Azure (and not during local runs).
- [ ] Migration name and baseline are specified (InitialCreate) and migration files will be committed to the repo.
- [ ] Rollback and migration failure behavior is described: how startup/liveness is affected, and whether the app should continue running in degraded mode or fail.
- [ ] Who owns running migrations manually (ops instructions) is documented for emergency or controlled deployments.

Resilience & Fault Handling
- [ ] Transient-fault handling and retry policy for DB operations are specified (e.g., EF Core retries or Polly) and thresholds are provided.
- [ ] Behavior during intermittent DB outages is specified (e.g., submission shows friendly error; operations are not silently dropped).

Security & Secrets
- [ ] The spec mandates storing connection strings in Azure App Settings or Key Vault, and explicitly forbids committing secrets to source control.
- [ ] Guidance is provided for using Key Vault references in App Settings if secret rotation is required.

Tests & CI
- [ ] It is explicit that DB-backed integration tests are NOT run in the default CI; method to opt-in (e.g., separate workflow) is described.
- [ ] Unit tests for DbRosterStore behavior (Add/GetAll) and model validation are included or planned.
- [ ] Integration test guidance includes steps for running against a disposable Azure MySQL instance or local emulator and how to enable them manually.

Observability & Logging
- [ ] The spec requires logging at Information level for successful connection/migration events and Error for failures; correlation IDs for user submissions are considered.
- [ ] Alerts or monitoring recommendations (e.g., Application Insights or metrics for DB errors and migration failures) are included.

Deployment & Rollout
- [ ] The recommended rollout path (deploy to staging slot with UseAzureDatabase=true and a test DB, verify, then swap) is clearly documented.
- [ ] The checklist specifies whether automatic migrations are acceptable for production or require manual approval.

Documentation & Developer Experience
- [ ] README and ops docs document how to enable DB mode (UseAzureDatabase), where to set MYSQLCONNSTR_localdb, and how to run migrations locally if needed.
- [ ] Local development remains simple: no DB required; instructions to opt into a local MySQL for dev are optional but documented if provided.

Edge Cases & Acceptance Criteria
- [ ] Acceptance tests are specified to verify persistence across restarts when running in Azure with DB enabled.
- [ ] Edge-case scenarios (malformed connection string, missing DB, migration failure) have acceptance behaviors defined and testable steps.

If any checklist item is unclear or cannot be satisfied by the current spec, annotate the item in the PR and request clarification from the feature author before implementation.

