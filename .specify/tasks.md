# Implementation Tasks: RosterVideo (feat/rostervideo-basic)

These tasks implement the RosterVideo feature per .specify/constitution.md and .specify/feature-spec.md. Tasks are ordered so each can be executed independently where possible. Follow the Test-First (spec-driven) principle: write tests/specs first where applicable, then implement.

1. Task ID: TSK-001 — Add RosterEntry model (P1)
   - Description: Create a POCO RosterEntry with Id (GUID), FirstName, LastName, Major, Shortcut, WhereUsed, CreatedAt.
   - Acceptance: Model compiles and has data annotations for required fields and reasonable length limits. Unit tests assert validation attributes exist.
   - Estimate: 0.5d

2. Task ID: TSK-002 — Implement IRosterStore + InMemoryRosterStore (P1)
   - Description: Define an interface IRosterStore with methods Add(RosterEntry) and IReadOnlyCollection<RosterEntry> GetAll(). Implement InMemoryRosterStore as a thread-safe singleton service registered in DI (AddSingleton).
   - Acceptance: Unit tests for Add and GetAll show expected behavior (newest-first ordering). Service is registered in Program.cs and can be injected into Razor Pages.
   - Estimate: 1d

3. Task ID: TSK-003 — Razor Page: Home (Index) with Form + List (P1)
   - Description: Create Pages/Index.cshtml and Index.cshtml.cs. The page shows the submission form and renders the list of entries (newest-first). Server-side validation must be enforced and invalid submissions return validation messages on the page.
   - Acceptance: Manual test — run the app, submit a valid entry, and it appears in the list. Tests for the PageModel validate that valid entries call IRosterStore.Add and invalid do not.
   - Estimate: 2d

4. Task ID: TSK-004 — JSON Feed Endpoint (P2)
   - Description: Add an endpoint at /api/roster (or /roster.json) that returns application/json with the list of roster entries. Implement as a minimal API endpoint in Program.cs or a Razor Pages API handler.
   - Acceptance: Integration test that requests /api/roster and asserts HTTP 200 and valid JSON containing entries.
   - Estimate: 1d

5. Task ID: TSK-005 — Input Validation & XSS Protection (P1)
   - Description: Enforce server-side validation (Required, StringLength) and ensure all rendered output is HTML-encoded. Add client-side unobtrusive validation for UX.
   - Acceptance: Unit tests for validation rules; manual inspection confirms encoded output for script-like input.
   - Estimate: 1d

6. Task ID: TSK-006 — Unit Tests for IRosterStore and RosterEntry Validation (P1)
   - Description: Add a test project (xUnit) and unit tests covering InMemoryRosterStore behavior and model validation rules.
   - Acceptance: Tests pass locally; included in CI pipeline.
   - Estimate: 1d

7. Task ID: TSK-007 — Basic Integration Tests for Home Page and JSON Feed (P2)
   - Description: Add simple integration tests (WebApplicationFactory or Playwright not required) to verify home page loads, form submission, and /api/roster returns JSON.
   - Acceptance: Tests run in CI and pass under dev environment assumptions.
   - Estimate: 1.5d

8. Task ID: TSK-008 — CI Pipeline Configuration (P2)
   - Description: Configure CI (GitHub Actions or Azure Pipelines) to run build, restore, unit tests, and critical integration tests. Add a simple publish artifact step for review deployments.
   - Acceptance: CI runs succeed on feature branch; PR gate configured to require successful checks before merge.
   - Estimate: 1d

9. Task ID: TSK-009 — Documentation: Run & Deploy (P2)
   - Description: Add README updates describing how to run locally in Visual Studio 2026, how to publish to Azure App Service, and expected behavior (in-memory store semantics). Include sample curl commands for the JSON feed.
   - Acceptance: README contains run instructions and a sample JSON request/response.
   - Estimate: 0.5d

10. Task ID: TSK-010 — Accessibility & UX polish (P3)
	- Description: Ensure basic accessibility (labels, ARIA where necessary), add empty-state messaging when no entries exist, and ensure responsive layout.
	- Acceptance: Manual validation against simple accessibility checklist (labels present, keyboard focus usable). Empty state displays on first load.
	- Estimate: 1d

Dependencies & Notes:
- TSK-001 -> TSK-002 -> TSK-003 -> TSK-004.
- Write unit tests (TSK-006) before implementing corresponding services (TSK-002) per constitution rules.
- Keep changes limited and incremental; target framework .NET 10 and Razor Pages. Use DI and singleton for in-memory store.

When you are ready for implementation, issue speckit.implement and I will begin making the code changes in the workspace following these tasks.
