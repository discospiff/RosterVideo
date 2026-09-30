# Implementation Tasks: InAppDatabase (branch: InAppDatabase)

These tasks implement the InAppDatabase feature per .specify/feature-spec-inappdatabase.md and the approved plan. Follow Test-First / Spec-Driven principles where applicable.

1. Task ID: IADB-001 — Add EF Core & MySQL provider packages (P1)
   - Description: Add PackageReference entries to RosterVideo/RosterVideo.csproj for:
	 - Microsoft.EntityFrameworkCore
	 - Microsoft.EntityFrameworkCore.Design
	 - Microsoft.EntityFrameworkCore.Tools
	 - Pomelo.EntityFrameworkCore.MySql
   - Acceptance: csproj updated and dotnet build succeeds. No code changes in this task beyond package refs.
   - Estimate: 0.25d

2. Task ID: IADB-002 — Add RosterDbContext (P1)
   - Description: Create RosterVideo/Data/RosterDbContext.cs with DbSet<RosterEntry> RosterEntries and minimal configuration (OnModelCreating optional). Ensure nullable annotations are compatible with existing model.
   - Acceptance: Compiles and unit tests referencing the context type can be created.
   - Estimate: 0.5d

3. Task ID: IADB-003 — Implement DbRosterStore : IRosterStore (P1)
   - Description: Implement a scoped DbRosterStore that uses RosterDbContext to Add and GetAll roster entries. Implement basic retry/transient-fault handling for Add operations.
   - Acceptance: Unit tests for Add/GetAll against an in-memory EF Core provider or SQLite in-memory pass (if applicable). The implementation adheres to IRosterStore contract.
   - Estimate: 1d

4. Task ID: IADB-004 — NormalizeAzureInAppConnString helper (P1)
   - Description: Add a helper method to parse MYSQLCONNSTR_localdb into a normalized MySQL connection string for Pomelo. Follow the referenced approach (StackOverflow) and do NOT assume port 3306. Handle malformed input with clear logging and fallback behavior.
   - Acceptance: Unit tests that feed example Azure In App connection strings (including non-3306 port) and assert expected normalized connection strings.
   - Estimate: 0.5d

5. Task ID: IADB-005 — Conditional DI & Startup registration (P1)
   - Description: Modify Program.cs to register services conditionally based on configuration.
	 - Default: AddSingleton<IRosterStore, InMemoryRosterStore>()
	 - If DB enabled (UseAzureDatabase==true OR (UseAzureDatabase unset AND WEBSITE_SITE_NAME present) AND normalized connection string available):
	   - AddDbContext<RosterDbContext>(opts => opts.UseMySql(conn, ServerVersion.AutoDetect(conn)))
	   - Register DbRosterStore as scoped and ensure it is used in place of the singleton.
   - Acceptance: App builds; when starting locally without UseAzureDatabase and MYSQLCONNSTR_localdb unset, the in-memory store remains active. Unit tests can validate DI registrations via ServiceProvider.
   - Estimate: 0.75d

6. Task ID: IADB-006 — Auto-apply migrations at startup when DB enabled (P1)
   - Description: When DB mode is enabled, apply EF Core migrations automatically at startup by creating a scope and calling Database.Migrate(). Log migration successes and failures.
   - Acceptance: Startup code compiles; when DB mode is simulated in tests, migration-call path is exercised (may be validated with a mocked DbContext or real test DB when enabled manually).
   - Estimate: 0.5d

7. Task ID: IADB-007 — Create initial migration "InitialCreate" (P1)
   - Description: Run dotnet ef migrations add InitialCreate (targeting Pomelo/MySQL provider) to generate migration files under RosterVideo/Migrations and commit them.
   - Acceptance: Migrations folder present and dotnet ef database update would apply schema (when connection string available). Migration files are included in repo commit.
   - Note: This step requires dotnet-ef tooling and working provider; if generation fails in the environment, document steps for maintainer to generate locally.
   - Estimate: 0.5d

8. Task ID: IADB-008 — Update CI & tests guidance (P2)
   - Description: Ensure default CI does not run DB-backed integration tests. Update .github/workflows/ci.yml and README with guidance for running DB-backed tests manually or in an opt-in workflow.
   - Acceptance: CI workflow unchanged for default tests; documentation updated explaining how to run DB tests manually.
   - Estimate: 0.25d

9. Task ID: IADB-009 — Add unit tests for Normalize helper and DbRosterStore (P2)
   - Description: Add unit tests validating normalization behavior, and basic DbRosterStore behavior using an in-memory EF core provider or SQLite in-memory for test-safety. These tests do NOT run in default CI if they require MySQL.
   - Acceptance: Tests pass locally without requiring Azure In App; DB-specific tests are separated or marked to run only when enabled.
   - Estimate: 1d

10. Task ID: IADB-010 — Docs & README updates (P2)
	- Description: Update README with instructions to enable UseAzureDatabase, how MYSQLCONNSTR_localdb is normalized, and how to obtain the publish profile. Document migration behavior and recommended rollout (staging slot, verify, swap).
	- Acceptance: README updated and contains command examples and required GitHub secrets for deployment.
	- Estimate: 0.25d

Dependencies & Notes
- Core dependency chain: IADB-001 -> IADB-002 -> IADB-003 -> IADB-005 -> IADB-006 -> IADB-007.
- Create migration (IADB-007) may require a local MySQL-compatible environment or workaround (use provider scaffolding); if not possible, include clear maintainer instructions.
- Keep DB-backed integration tests opt-in and off by default per the spec.

When you're ready for implementation, issue speckit.implement and I will execute these tasks in the workspace.

11. Task ID: IADB-011 — Add failing unit tests (Test-First) (P1)
   - Description: Create failing unit tests (red) for NormalizeAzureInAppConnString and DbRosterStore behavior before implementing the corresponding code. Tests should assert expected normalized connection strings and that DbRosterStore calls SaveChangesAsync when Add is called.
   - Acceptance: Tests compile and fail (red) initially; they will be used to drive implementation.
   - Estimate: 0.25d

12. Task ID: IADB-012 — Define and implement retry policy (P1)
   - Description: Add a concrete transient-fault retry policy for DB operations: 3 retries, exponential backoff starting at 200ms with jitter, and logging on each retry. Implement via EF Core execution strategy or Polly as appropriate and ensure Add operations use the policy.
   - Acceptance: Unit tests simulate transient failures and verify retries occur and final behavior is correct.
   - Estimate: 0.5d

13. Task ID: IADB-013 — Opt-in CI job for DB-backed integration tests (P2)
   - Description: Add a separate GitHub Actions workflow (e.g., .github/workflows/db-integration.yml) or a workflow_dispatch path that runs DB-backed integration tests when explicitly triggered with required secrets. Document how to trigger it and required secrets (test DB connection string).
   - Acceptance: Workflow skeleton present and documented; it does not run by default on push PR.
   - Estimate: 0.5d

14. Task ID: IADB-014 — Logging event names & levels specification (P2)
   - Description: Add a small logging spec that defines event names and levels for key DB events: DBConnection_Attempt (Information), DBConnection_Success (Information), DBConnection_Failure (Error), Migrations_Start (Information), Migrations_Success (Information), Migrations_Failure (Error), DB_Add_Retry (Warning/Information depending on occurrence).
   - Acceptance: Spec added to repo (.specify/logging-inappdatabase.md) and implementation will emit these events.
   - Estimate: 0.25d
