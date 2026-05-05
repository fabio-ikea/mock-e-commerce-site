---
applyTo: '**'
---

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

## Development Patterns
- **Components**: Reusable, prop-typed, hook-based
- **Hooks**: Data fetching (useProducts), state management
- **Services**: Inject dependencies; use in-memory implementations
- **Endpoints**: RESTful; minimal API handlers in Endpoints/

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
