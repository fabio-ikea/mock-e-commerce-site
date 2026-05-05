# Implementation Plan: Manage Cart

**Branch**: `main` | **Date**: 2026-05-05 | **Spec**: `specs/001-manage-cart/spec.md`
**Input**: Feature specification from `/specs/001-manage-cart/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement a cart review and quantity-management flow that uses the existing cart
API as the source of truth for item counts and totals. The work will complete the
in-memory cart service and cart endpoints, add a `PUT /api/cart/{productId}`
quantity replacement contract, return cart-summary data needed for pricing
review, and add a frontend cart panel accessible from the existing header icon.
The feature must enforce the per-product quantity cap of 5, keep rejected
mutations side-effect free, and add matching frontend and backend tests.

## Technical Context

**Language/Version**: C# / .NET 10.0 backend; TypeScript 6 with React 19 frontend  
**Primary Dependencies**: ASP.NET Core Minimal APIs, Microsoft.AspNetCore.OpenApi, React 19, Vite 8, Vitest 4, React Testing Library, Fetch API  
**Storage**: In-memory singleton cart service plus mock in-memory product catalog  
**Testing**: xUnit with `WebApplicationFactory` for backend; Vitest + React Testing Library for frontend  
**Target Platform**: Modern desktop browsers with a local .NET web API backend
**Project Type**: Full-stack web application  
**Performance Goals**: Cart review and quantity changes should update visible cart state within a single user interaction for demo-sized carts without noticeable delay  
**Constraints**: Max quantity of 5 per product; PUT updates existing cart items only; invalid requests leave the cart unchanged; cart opens from the existing header icon; user-facing controls must be keyboard accessible  
**Scale/Scope**: Single shared demo cart, small mock catalog, one cart review surface, one new REST mutation, and matching automated coverage across frontend and backend

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] Full-stack contract impact is identified for `CartEndpoints.cs`,
  `ICartService.cs`, `InMemoryCartService.cs`, backend cart models/requests,
  `src/frontend/src/api/index.ts`, `src/frontend/src/types/index.ts`, `App.tsx`,
  `Header.tsx`, and new cart hook/component files.
- [x] Test strategy covers affected layers with backend endpoint/service tests,
  frontend cart component and hook tests, and contract-aware assertions around
  GET/POST/PUT cart responses.
- [x] Simplicity is preserved by keeping validation in the service/API layer,
  fetch logic in the API layer and cart hook, and presentation concerns inside
  dedicated cart UI components.
- [x] Security and observability needs are addressed through quantity validation,
  consistent rejection responses, explicit not-found handling, and user-visible
  error states without leaking sensitive details.
- [x] User experience expectations cover loading, empty, success, and error
  states plus keyboard accessibility for opening, reviewing, and updating the cart.

**Post-Design Gate Review**: PASS — `research.md`, `data-model.md`,
`contracts/cart-api.yaml`, and `quickstart.md` resolve the quantity semantics,
error behavior, and UI expectations without requiring constitution exceptions.

## Project Structure

### Documentation (this feature)

```text
specs/001-manage-cart/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/
│   └── cart-api.yaml    # Phase 1 API contract
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/
├── backend/
│   └── MockEcommerce.Api/
│       ├── Program.cs
│       ├── Endpoints/
│       │   └── CartEndpoints.cs
│       ├── Models/
│       │   ├── CartItem.cs
│       │   └── Product.cs
│       └── Services/
│           ├── ICartService.cs
│           ├── InMemoryCartService.cs
│           └── IProductService.cs
└── frontend/
    └── src/
        ├── api/
        │   └── index.ts
        ├── components/
        │   ├── Header/
        │   │   └── Header.tsx
        │   └── Cart/                 # new cart presentation components
        ├── hooks/
        │   ├── useProducts.ts
        │   └── useCart.ts            # new cart state hook
        ├── types/
        │   └── index.ts
        └── App.tsx

test/
├── backend/
│   └── MockEcommerce.Api.Tests/
│       ├── Endpoints/
│       │   └── CartEndpointTests.cs
│       └── Services/
│           └── InMemoryCartServiceTests.cs
└── frontend/
    ├── App.test.tsx
    ├── components/
    │   ├── Header/
    │   │   └── Header.test.tsx
    │   └── Cart/
    │       └── CartPanel.test.tsx
    └── hooks/
        └── useCart.test.ts
```

**Structure Decision**: Use the existing full-stack web application layout. The
backend change set stays inside `src/backend/MockEcommerce.Api/` and its test
project, while the frontend change set extends `src/frontend/src/` with a small
cart-specific hook and presentation component reachable from `App.tsx` and
`Header.tsx`.

## Complexity Tracking

> No constitution violations are expected for this feature.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |
