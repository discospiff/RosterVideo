# RosterVideo Constitution

## Core Principles

### I. Code Quality (NON-NEGOTIABLE)
Every production change must be clear, idiomatic C# targeting .NET 10, well-factored, and easy to reason about. Code must follow the project's style guide and established naming conventions. PRs must include concise rationale for complex decisions and justify deviations from standard patterns.

### II. Test-First (SPEC-DRIVEN) (NON-NEGOTIABLE)
All new features and behavioral changes begin with a specification and failing tests (unit and/or integration) that express the expected behavior. Follow Red-Green-Refactor: write spec/tests → run and see failures → implement minimal code to pass tests → refactor with tests green. Tests must be deterministic and fast where possible.

### III. Testing Standards
- Unit tests cover business logic and keep external dependencies mocked.
- Integration tests verify contracts across boundaries (data access, file I/O, HTTP, and external APIs).
- End-to-end tests cover critical user journeys (login, upload, playback) and run in CI for release-critical branches.
- Test naming must follow "MethodUnderTest_Condition_ExpectedResult" or the project's agreed convention.

### IV. User Experience Consistency
UX must be consistent across pages: consistent layout, typography, color usage, and interaction patterns. Errors and validations should be actionable and localized. Accessibility (WCAG AA) is a requirement for all public-facing flows and primary features.

### V. Performance & Scalability
Set baseline performance expectations: page loads under 1s for primary pages on representative test hardware/connection; key APIs p95 latency under 250ms under normal load. Measure performance with benchmarks and profiling; avoid premature optimization but optimize after measurement. Design for graceful degradation under load.

### VI. Observability & Security
Structured logging, correlation IDs for requests, and meaningful metrics are required for server-side components. Secrets must never be committed and must be stored in environment-specific secret stores. Follow OWASP guidelines for input validation, output encoding, and authentication/authorization.

## Additional Constraints
- Target framework: .NET 10 for all new projects and libraries unless a valid exception is documented.
- Prefer Razor Pages for UI features; Blazor or MVC may be used only with justification.
- Use dependency injection and avoid service locators.

## Development Workflow & Quality Gates
- All work flows through feature branches and pull requests with at least one approving review from a team member other than the author.
- CI must run: build, restore, linting/static analysis, unit tests, and critical integration tests before merging to main/master.
- Merge is blocked if tests fail, code coverage drops below agreed thresholds, or critical static analysis rules fail.

## Governance
This constitution supersedes ad-hoc practices. Amendments require a documented proposal, at least one design review, and a migration plan when applicable. Owners of major subsystems should maintain short guidance docs linked from this constitution.

**Version**: 1.0.0 | **Ratified**: 2026-09-02 | **Last Amended**: 2026-09-02
