# Logging spec: InAppDatabase

Event names and levels for database and migration events:

- DBConnection_Attempt (Information) — when the application attempts to connect to the database during startup.
- DBConnection_Success (Information) — when a connection to the database is successfully established.
- DBConnection_Failure (Error) — when a connection attempt fails; include exception details and correlation id.
- Migrations_Start (Information) — when automatic migration application begins.
- Migrations_Success (Information) — when migrations applied successfully; include applied migration count or list.
- Migrations_Failure (Error) — when migrations fail; include exception details and recommended remediation.
- DB_Add_Retry (Warning) — when a retry is attempted for Add operations; include retry count and delay.
- DB_Add_Failure (Error) — when Add ultimately fails after retries.

Structured logging properties:
- CorrelationId (string) — request-scoped identifier propagated from HTTP context for user submissions.
- UserIp (string) — optional remote IP when available.
- EntryId (GUID) — the Id of the roster entry associated with the log event (if applicable).
