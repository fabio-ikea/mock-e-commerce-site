<!--
Sync Impact Report
- Version change: template -> 1.0.0
- Modified principles:
	- Template principle 1 -> I. Full-Stack Contract Alignment
	- Template principle 2 -> II. Test-Backed Delivery (NON-NEGOTIABLE)
	- Template principle 3 -> III. Simplicity and Single Responsibility
	- Template principle 4 -> IV. Secure and Observable Defaults
	- Template principle 5 -> V. Accessibility and User Experience Quality
- Added sections:
	- Engineering Standards
	- Delivery Workflow and Quality Gates
- Removed sections:
	- None
- Templates requiring updates:
	- ✅ .specify/templates/plan-template.md
	- ✅ .specify/templates/spec-template.md
	- ✅ .specify/templates/tasks-template.md
	- ✅ .github/copilot-instructions.md
- Follow-up TODOs:
	- None
-->

# Mock E-Commerce Site Constitution

## Core Principles

### I. Full-Stack Contract Alignment
Every feature MUST preserve a clear contract between backend endpoints, frontend
API helpers, React hooks, and UI components. Backend request and response shapes
MUST be reflected in TypeScript types, and frontend data access MUST flow through
the API layer rather than direct `fetch` calls inside components. Changes to
models, endpoints, or service interfaces MUST update all affected tests and
consumer code in the same change set.

### II. Test-Backed Delivery (NON-NEGOTIABLE)
All behavior changes MUST be accompanied by automated tests at the appropriate
layer before the work is considered complete. Frontend changes MUST add or update
Vitest and React Testing Library coverage for rendering, states, and interactions.
Backend changes MUST add or update XUnit coverage for endpoints and services.
Contract changes between frontend and backend MUST be validated end-to-end.

### III. Simplicity and Single Responsibility
Components, hooks, services, endpoints, and models MUST each have one clear
reason to change. React code MUST use functional components and hooks only, with
state kept as local as practical. Backend code MUST use dependency injection,
interface-based services, and asynchronous APIs where I/O or external boundaries
exist. New abstractions are permitted only when they reduce duplication or clarify
behavior better than a direct implementation.

### IV. Secure and Observable Defaults
Input validation, predictable error handling, and safe defaults are required for
all user-facing and API-facing changes. Cross-origin access, configuration, and
future persistence integrations MUST be explicit rather than implied. Failures and
unexpected states MUST surface actionable diagnostics for developers, while users
receive clear non-sensitive messages. Logging and error reporting MUST be added
when the feature introduces meaningful operational risk.

### V. Accessibility and User Experience Quality
User-facing functionality MUST account for loading, empty, success, and error
states. Frontend work MUST prefer semantic HTML, keyboard accessibility, and
clear labels over purely visual implementations. UI behavior MUST remain usable
without hidden assumptions about timing or device type. A feature is incomplete if
it works only in the happy path or only for mouse users.

## Engineering Standards

The canonical stack for this repository is React 19+ with TypeScript, Vite,
Vitest, and React Testing Library on the frontend, and .NET 10 minimal APIs with
XUnit on the backend. TypeScript strict mode MUST remain enabled. Components and
classes MUST use PascalCase; functions, hooks, and variables MUST use camelCase;
service interfaces MUST use the `IServiceName` pattern. Frontend code MUST keep
API logic in `src/frontend/src/api/`, reuse typed hooks for data loading, and keep
presentation components focused. Backend code MUST keep endpoint mapping in
`Endpoints/`, business behavior in `Services/`, and transport models explicit in
`Models/`. In-memory implementations are acceptable defaults until a feature
specification explicitly requires persistence.

## Delivery Workflow and Quality Gates

Specifications MUST define prioritized user stories, edge cases, functional
requirements, and measurable success criteria. Plans MUST record the real project
structure, technical context, and a Constitution Check that verifies contract
alignment, test coverage, accessibility expectations, and security/error handling.
Tasks MUST be organized by user story and include any required frontend,
backend, integration, accessibility, and documentation work. Before merge,
contributors MUST run the relevant automated tests and any affected build or lint
commands. A change that breaks `npm test`, frontend build expectations, or
`dotnet test` is not ready for review.

## Governance

This constitution supersedes conflicting local conventions and serves as the
source of truth for feature planning, implementation, and review. Amendments MUST
be made through a documented pull request that updates this file and any affected
templates or guidance files in the same change. Versioning follows semantic
versioning for governance: MAJOR for incompatible principle changes or removals,
MINOR for new principles or materially expanded requirements, and PATCH for
clarifications that do not alter expectations. Every review MUST include an
explicit compliance check against this constitution, the active specification,
and `.github/copilot-instructions.md` when AI-assisted generation is used.

**Version**: 1.0.0 | **Ratified**: 2026-05-05 | **Last Amended**: 2026-05-05
