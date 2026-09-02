# Feature Specification: RosterVideo

**Feature Branch**: `feat/rostervideo-basic`

**Created**: 2026-09-02

**Status**: Draft

**Input**: User description: "A simple roster web app where users submit first name, last name, major, favorite keyboard shortcut and where it is used. Submissions are stored in memory, listed on the home page, and exposed as a JSON feed on a separate page. App runs locally in Visual Studio 2026 and will be deployed to Azure. Uses Razor Pages on .NET 10."

## User Scenarios & Testing (mandatory)

### User Story 1 - Submit roster entry (Priority: P1)

As a site visitor, I want to fill out a small form on the home page with my first name, last name, major, favorite keyboard shortcut, and where that shortcut can be used, so that my entry is recorded for others to see.

Why this priority: Core functionality and primary value of the application — captures roster data.

Independent Test: Open the home page, fill the form fields with valid values, submit, and verify the new entry appears in the list on the same page.

Acceptance Scenarios:
1. Given the home page is loaded, When the required fields are filled and the form is submitted, Then the entry is stored in memory and displayed in the roster list on the home page.
2. Given a required field is missing or invalid (empty), When the form is submitted, Then the page shows a validation error and the entry is not saved.

---

### User Story 2 - View roster list (Priority: P1)

As a site visitor, I want to see a list of all roster entries on the home page so I can learn about other users' submissions.

Why this priority: Primary read visibility for submissions.

Independent Test: After one or more entries are submitted, reload or navigate to the home page and verify entries are rendered in a simple table or list showing all submitted fields.

Acceptance Scenarios:
1. Given there are saved entries in memory, When the home page is visited, Then all entries are displayed in reverse chronological order (newest first) showing first name, last name, major, shortcut, and where.

---

### User Story 3 - JSON feed (Priority: P2)

As a developer or consumer, I want a separate page that returns the roster entries as JSON so I can programmatically consume the data.

Why this priority: Useful for integrations and verifying stored data; not required for basic manual usage.

Independent Test: Navigate to /api/roster (or /roster.json) and verify the response is application/json and contains an array of entries matching the fields stored in memory.

Acceptance Scenarios:
1. Given there are saved entries, When the JSON endpoint is requested, Then the server responds with HTTP 200 and a JSON array of roster entries.

---

### User Story 4 - Local / Azure run and dev experience (Priority: P1)

As a developer, I want the app to run locally in Visual Studio 2026 and be deployable to Azure App Service so development and deployment are straightforward.

Why this priority: Ensures the development and release workflow is practical for the team.

Independent Test: Developer opens the solution in Visual Studio 2026, runs the app with IIS Express or Kestrel, and the home page loads. A simple deployment pipeline or Azure publish profile allows publishing to Azure App Service.

Acceptance Scenarios:
1. Given Visual Studio 2026, When the solution is launched (F5), Then the Razor Pages app loads and the home page is reachable.
2. Given an Azure publish profile or `dotnet publish`, When the app is published to Azure App Service, Then the home page and JSON feed are reachable in the deployed environment.

---

### Edge Cases

- Submissions when memory store is empty — home page shows an appropriate empty state message.
- Server restart clears in-memory data — document as expected behavior for this iteration.
- Malicious input (scripts) should be encoded on output to prevent XSS.
- Large input values: enforce reasonable length limits on text fields and validate on submit.

## Requirements (mandatory)

### Functional Requirements

- FR-001: Home page MUST contain an HTML form with fields: FirstName (required), LastName (required), Major (optional), Shortcut (required), WhereUsed (optional).
- FR-002: Submitted entries MUST be stored in an in-memory store (singleton service) for the lifetime of the application process.
- FR-003: Home page MUST render a list of all saved entries (newest-first) showing FirstName, LastName, Major, Shortcut, WhereUsed.
- FR-004: System MUST expose a JSON feed endpoint (e.g., /api/roster or /roster.json) returning the same entries as JSON with content-type application/json.
- FR-005: Input fields MUST be validated server-side; client-side validation is recommended for UX but not a substitute.
- FR-006: Output MUST HTML-encode data rendered on pages to prevent XSS.
- FR-007: The app MUST target .NET 10 and use Razor Pages as the primary UI framework.
- FR-008: The app MUST run locally in Visual Studio 2026 (IIS Express/Kestrel) and be deployable to Azure App Service.

### Key Entities

- RosterEntry: { Id: GUID, FirstName: string, LastName: string, Major: string?, Shortcut: string, WhereUsed: string?, CreatedAt: DateTime }

## Success Criteria (mandatory)

### Measurable Outcomes

- SC-001: A new roster entry can be created via the home page form and appears on the page within 2 seconds of submission in a local dev environment.
- SC-002: The JSON feed returns the same entries and validates as proper JSON (e.g., using jq) with HTTP 200.
- SC-003: The app starts and serves the home page in Visual Studio 2026 with no runtime errors on first load.
- SC-004: Basic input validation prevents empty required fields; acceptance tests cover these scenarios.

## Assumptions

- The initial persistence layer is in-memory only; persistence to a database is out of scope for this feature.
- Authentication and authorization are out of scope; the app is public for v1.
- Multi-user concurrency is limited to development/testing loads; no production-scale concurrency guarantees for in-memory store.
- Deployment to Azure App Service is standard and will use the recommended .NET publish workflows or VS publish profile.
