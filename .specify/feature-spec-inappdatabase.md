# Feature Specification: In-App Database for RosterVideo

**Feature Branch**: `InAppDatabase`

**Created**: 2026-09-02

**Status**: Draft

**Input**: User description: "When the app is deployed and running in Azure App Service, roster submissions should be persisted to a database. When the app runs locally (Visual Studio or dotnet run) it should continue to use the existing in-memory store."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Persist in Azure (Priority: P1)

As a user of the deployed site, when I submit a roster entry in the Azure-hosted app, I want the entry to be stored durably so that it survives app restarts and scale-out events.

Why this priority: Durable storage is required for production use so user data is not lost between restarts.

Independent Test: Deploy the app to an Azure App Service configured with the required connection string and set UseAzureDatabase=true (or rely on the presence of WEBSITE_SITE_NAME). Submit an entry via the home page, then restart the App Service and verify the entry still appears.

Acceptance Scenarios:
1. Given the app is deployed to Azure and configured to use the database, When a valid roster entry is submitted, Then the entry is saved to the persistent database and survives an application restart.
2. Given the app is deployed but database connectivity fails, When a submission is attempted, Then the app returns a graceful error and logs the failure; consider optionally queuing the entry if desired.

---

### User Story 2 - Local/dev uses In-Memory (Priority: P1)

As a developer running the app locally in Visual Studio, I want submissions to continue using the existing in-memory store so I can iterate quickly without provisioning a database.

Why this priority: Developer experience and safety — no accidental writes to production DB during development.

Independent Test: Run the app locally (dotnet run or F5) without Azure-specific app settings. Submit entries and verify they are stored in-memory and are lost when the dev process stops.

Acceptance Scenarios:
1. Given the app is running locally with no Azure DB configuration, When a submission is made, Then the in-memory store is used and entries do not persist across restarts.

---

### User Story 3 - Configuration & Safe Detection (Priority: P1)

As an operator, I want deterministic control over whether the app persists to the database (explicit flag) and a safe fallback detection for Azure hosting so accidental persistence in non-Azure environments is avoided.

Why this priority: Prevents accidental writes and makes behavior explicit.

Independent Test: Start the app with environment variable UseAzureDatabase=true and a valid connection string; verify DB persistence. Start with UseAzureDatabase=false but on Azure (WEBSITE_SITE_NAME present); verify precedence is documented and behavior is as designed.

Acceptance Scenarios:
1. Given app setting UseAzureDatabase=true and a valid connection string, When the app starts, Then it uses the database store.
2. Given app setting UseAzureDatabase is not present, When the app is running on Azure App Service (WEBSITE_SITE_NAME exists) and a connection string is present, Then the app uses the database store.

---

### Edge Cases

- Partial failures: database is reachable at startup but becomes unavailable later; the app should handle exceptions, log them, and surface a friendly message on submission failure.
- Migrations: schema migrations must be applied on deployment. Decide whether to apply migrations automatically at startup in Azure or require a separate release step.
- Secrets: connection strings must be stored in Azure App Settings or Key Vault; never commit them to source control.

## Requirements *(mandatory)*

### Functional Requirements

- FR-001: The system MUST persist roster entries to a durable database only when running in Azure (per detection rules), otherwise use the existing in-memory IRosterStore implementation.
- FR-002: The data model persisted MUST match the RosterEntry entity shape used by the in-memory store.
- FR-003: The application MUST support configuration via appsettings and environment variables. The following keys will be used:
  - ConnectionStrings:DefaultConnection (standard connection string)
  - UseAzureDatabase (boolean; optional explicit override)
- FR-004: The DI container MUST register either the InMemoryRosterStore (AddSingleton) or a Db-backed implementation (scoped DbContext + scoped store) based on configuration at startup.
- FR-005: Schema migrations MUST be applied during Azure deployments. Provide an administrator-controlled option to apply migrations automatically at startup when UseAzureDatabase=true and environment indicates Azure.
- FR-006: The app MUST log database connectivity and migration events at Information level and errors at Error level.
- FR-007: The home page behavior (form and listing) MUST be unchanged from the user's perspective; only the persistence backend changes.

### Non-Functional Requirements

- NFR-001: Secrets (connection strings) MUST be stored in Azure App Settings or Key Vault and not in source control.
- NFR-002: DB operations MUST be resilient; apply transient-fault handling/retries for production DB calls.
- NFR-003: Local developer experience MUST be preserved: no DB required, no migration steps needed.

## Key Entities *(include if feature involves data)*

- RosterEntry: { Id: GUID, FirstName: string, LastName: string, Major: string?, Shortcut: string, WhereUsed: string?, CreatedAt: DateTime }

## Success Criteria *(mandatory)*

### Measurable Outcomes

- SC-001: When deployed to Azure and configured to use the database, entries submitted via the home page persist across app restarts in the deployed environment.
- SC-002: Running locally with no Azure configuration continues to use the in-memory store; submissions are ephemeral.
- SC-003: CI pipeline can run database-related unit/integration tests in a way that does not require Azure unless explicitly opted-in.

## Assumptions

- Primary production DB will be MySQL In App on Azure App Service (use Pomelo.EntityFrameworkCore.MySql provider). EF Core will be used for data access (provider pluggable).
- The app will detect Azure environment by an explicit config key UseAzureDatabase (preferred) or by the presence of WEBSITE_SITE_NAME (fallback). An explicit override variable takes precedence as defined in the detection rules.
- Applying migrations automatically at startup is acceptable for the initial rollout when running in Azure and DB mode is enabled; control for auto-apply is provided by an environment variable MIGRATIONS_AUTOAPPLY (boolean) and defaults to true only in Azure when UseAzureDatabase=true.

### Example Azure In App connection string (for tests and normalization)

Azure may expose a connection string in the environment variable MYSQLCONNSTR_localdb with a format such as:

- "Database=localdb;Data Source=tcp:127.0.0.1,52137;User Id=localdb;Password=ExamplePwd;"

The NormalizeAzureInAppConnString helper MUST convert the above into a Pomelo/MySql-compatible connection string, for example:

- "Server=127.0.0.1;Port=52137;Database=localdb;User=localdb;Password=ExamplePwd;"

The helper must not assume port 3306 and must extract the dynamic port from the Data Source / tcp host value.

## Implementation Notes (for the later speckit.implement)

- Add a new DbContext (RosterDbContext) and an EF Core-backed RosterStore implementing IRosterStore. The Db-backed store will be registered when UseAzureDatabase=true or when WEBSITE_SITE_NAME exists and a connection string is configured.
- Use dependency injection to choose implementation at startup: builder.Services.AddSingleton<IRosterStore, InMemoryRosterStore>() by default; if DB enabled, register DbContext and a DbRosterStore with scoped lifetime and replace the singleton registration.
- Apply EF Core migrations on startup only when DB is enabled and running in Azure. Log the migration actions.
- Protect the connection string via standard Configuration and Azure App Settings; recommend using Key Vault for secrets.
- Add integration test guidance: create a separate pipeline/job that can run DB-backed integration tests against a disposable Azure SQL instance or localdb when explicitly enabled.

## Governance & Rollout

- Feature should be rolled out behind configuration. Start by deploying to a staging slot with UseAzureDatabase=true and a test database, verify persistence and migrations, then swap to production.
- Monitor logs and errors related to migration and DB connectivity during rollout.
