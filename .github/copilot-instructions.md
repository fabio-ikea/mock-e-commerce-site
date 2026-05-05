---
applyTo: '**'
---

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan:
`specs/001-manage-cart/plan.md`
<!-- SPECKIT END -->

# Mock E-Commerce Site — Copilot Instructions

## Architecture
- **Frontend**: React 19+ with TypeScript, Vite, Vitest
- **Backend**: .NET 10.0 Minimal APIs, C#, XUnit
- **API**: RESTful endpoints (products, cart) via Fetch API

## Code Style & Conventions
- TypeScript: strict mode, PascalCase components, camelCase functions
- React: functional components, hooks only (no Redux), single-responsibility
- C#: async/await patterns, interface-based services, dependency injection
- Naming: `IServiceName` interfaces, `Service` implementations

## Testing Standards
- **Frontend**: Vitest + React Testing Library; mirror src structure
- **Backend**: XUnit with mocked services
- Maintain test parity across full stack
- Update tests for every behavior change; include integration coverage for API contract changes

## Development Patterns
- **Components**: Reusable, prop-typed, hook-based
- **Hooks**: Data fetching (useProducts), state management, error/loading handling
- **API Layer**: Keep `fetch` calls in `src/frontend/src/api/`, not in UI components
- **Services**: Inject dependencies; use in-memory implementations
- **Endpoints**: RESTful; minimal API handlers in Endpoints/

## Quality Expectations
- Model frontend and backend changes as one contract-aware feature
- Handle loading, empty, success, and error states in user-facing flows
- Prefer semantic HTML, keyboard accessibility, and clear labels
- Add validation, predictable error handling, and useful diagnostics

## Common Workflows
1. Add feature: Endpoint → Service → Component → Tests
2. Modify data model: Update Models/, Services, Endpoints, types
3. New component: Create in components/, add hook if needed, test

## Build Commands
- Frontend: `npm run dev`, `npm run build`, `npm test`
- Backend: `dotnet run`, `dotnet build`, `dotnet test`

## Best Practices
- Document complex business logic
- Keep files focused and single-responsibility
- Use async/await consistently
- Always write tests alongside features
