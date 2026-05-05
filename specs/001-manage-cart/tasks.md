---
description: "Task list for Manage Cart feature"
---

# Tasks: Manage Cart

**Input**: Design documents from `/specs/001-manage-cart/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/cart-api.yaml ✅

**Tests**: Test tasks are REQUIRED. This feature changes behavior across backend
service, API endpoints, frontend API layer, hooks, and UI components — all
layers need matching automated coverage.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Verify existing model scaffolding and create the shared cart response
model that every endpoint and test will reference.

- [X] T001 Verify `src/backend/MockEcommerce.Api/Models/CartItem.cs` fields match the data-model: `ProductId`, `ProductName`, `UnitPrice`, `Quantity`, and `TotalPrice` (computed as `UnitPrice * Quantity`)
- [X] T002 Create `src/backend/MockEcommerce.Api/Models/CartSummary.cs` with `IEnumerable<CartItem> Items`, `int ItemCount`, and `decimal Subtotal` properties — this is the unified response shape for all cart endpoints

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared contracts, backend interface additions, and frontend type/API
helpers that ALL user story phases require before implementation can begin.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T003 Create `src/backend/MockEcommerce.Api/Models/UpdateCartQuantityRequest.cs` with a single `int Quantity` property — required by the PUT endpoint in US2
- [X] T004 Add `UpdateQuantity(int productId, int quantity)` method signature to `src/backend/MockEcommerce.Api/Services/ICartService.cs` — must return a result that communicates success, not-found, or validation failure
- [X] T005 [P] Add `CartItem`, `CartSummary`, and `UpdateCartQuantityRequest` TypeScript interfaces to `src/frontend/src/types/index.ts` — map each field from `data-model.md`; `CartItem.totalPrice` is `unitPrice * quantity`
- [X] T006 [P] Add `fetchCart(): Promise<CartSummary>` and `updateCartQuantity(productId: number, request: UpdateCartQuantityRequest): Promise<CartSummary>` to `src/frontend/src/api/index.ts` — keep fetch calls confined to this module per project conventions

**Checkpoint**: Shared C# models, updated service interface, and frontend API
helpers are in place — user story implementation can now begin in parallel with
frontend and backend tracks.

---

## Phase 3: User Story 1 — Review Cart Contents (Priority: P1) 🎯 MVP

**Goal**: Shoppers can open the cart from the header icon and see all items with
name, quantity, unit price, line total, and overall subtotal. An empty-cart
state is shown when no items have been selected.

**Independent Test**: POST a product via `/api/cart`, then open the cart from the
header icon — verify item name, quantity, unit price, line total, and subtotal
are all displayed correctly. Open the cart before adding anything and verify the
empty-cart message is shown.

### Tests for User Story 1 ⚠️

> **Write these tests FIRST — they should FAIL before the implementation tasks run**

- [X] T007 [P] [US1] Create `test/backend/MockEcommerce.Api.Tests/Services/InMemoryCartServiceTests.cs` — cover `GetAll` returning an empty list on a fresh store and `Add` persisting a new `CartItem` with correct `ProductName`, `UnitPrice`, and `Quantity`
- [X] T008 [P] [US1] Create `test/backend/MockEcommerce.Api.Tests/Endpoints/CartEndpointTests.cs` — cover `GET /api/cart` returning `200` with an empty `CartSummary`, and `POST /api/cart` returning `201` with a populated `CartSummary` after a successful add
- [X] T009 [P] [US1] Create `test/frontend/hooks/useCart.test.ts` — cover initial cart load from `fetchCart`, `loading` transitioning to `false`, and the returned `cart.items` matching the API response
- [X] T010 [P] [US1] Create `test/frontend/components/Cart/CartPanel.test.tsx` — cover rendering each item's `productName`, `quantity`, `unitPrice`, and `totalPrice`; rendering `subtotal`; and rendering the empty-cart message when `items` is an empty array
- [X] T011 [P] [US1] Update `test/frontend/components/Header/Header.test.tsx` — verify that clicking the existing cart icon button invokes the provided `onCartOpen` handler prop

### Implementation for User Story 1

- [X] T012 [US1] Implement `GetAll()` in `src/backend/MockEcommerce.Api/Services/InMemoryCartService.cs` — return the current in-memory `IEnumerable<CartItem>`; an empty list for a fresh cart
- [X] T013 [US1] Implement `Add(AddToCartRequest request)` in `src/backend/MockEcommerce.Api/Services/InMemoryCartService.cs` — look up product details via `IProductService`, create a new `CartItem` or increment an existing item's quantity, and return the added or updated `CartItem`
- [X] T014 [US1] Update the `GET /api/cart` handler in `src/backend/MockEcommerce.Api/Endpoints/CartEndpoints.cs` — call `GetAll()`, build a `CartSummary` (set `ItemCount` to the sum of item quantities and `Subtotal` to the sum of `TotalPrice`), and return `200 OK`
- [X] T015 [US1] Update the `POST /api/cart` handler in `src/backend/MockEcommerce.Api/Endpoints/CartEndpoints.cs` — call `Add()`, build and return the updated `CartSummary`; return `201 Created` for a new item, `200 OK` when an existing item is incremented
- [X] T016 [P] [US1] Create `src/frontend/src/hooks/useCart.ts` — expose `cart: CartSummary`, `loading: boolean`, `error: string | null`, and a `refresh()` function backed by `fetchCart()`; call `fetchCart()` on mount
- [X] T017 [P] [US1] Create `src/frontend/src/components/Cart/CartPanel.tsx` — render a list of cart items (name, quantity, unit price, line total), the overall `subtotal`, and an empty-cart message when `cart.items` is empty; accept `cart`, `loading`, `error`, and `onClose` props
- [X] T018 [US1] Create `src/frontend/src/components/Cart/index.ts` — barrel-export `CartPanel` to match project component conventions
- [X] T019 [P] [US1] Update `src/frontend/src/components/Header/Header.tsx` — accept an `onCartOpen: () => void` prop and wire it to the existing cart icon button's `onClick` handler
- [X] T020 [US1] Update `src/frontend/src/App.tsx` — add `cartOpen` boolean state, pass `onCartOpen` to `Header`, conditionally render `CartPanel` with the `useCart` hook's values, and pass `onClose` to close the panel

**Checkpoint**: User Story 1 is fully functional — shoppers can add items and
review the cart from the header icon, including an empty-cart state.

---

## Phase 4: User Story 2 — Adjust Selected Quantities (Priority: P2)

**Goal**: Shoppers can change the quantity of an item already in their cart from
within the cart panel and see updated line totals and subtotal immediately.

**Independent Test**: Add an item to the cart, open the cart panel, change its
quantity to a different valid value (1–5), and verify the updated quantity, line
total, subtotal, and header item count are all reflected.

### Tests for User Story 2 ⚠️

- [X] T021 [P] [US2] Add `UpdateQuantity` success tests to `test/backend/MockEcommerce.Api.Tests/Services/InMemoryCartServiceTests.cs` — cover updating to a new valid quantity (1, 3, 5) and verifying the stored `CartItem.Quantity` changes
- [X] T022 [P] [US2] Add `PUT /api/cart/{productId}` success tests to `test/backend/MockEcommerce.Api.Tests/Endpoints/CartEndpointTests.cs` — cover `200 OK` response with the updated `CartSummary` shape after a valid quantity replacement
- [X] T023 [P] [US2] Add `updateQuantity` tests to `test/frontend/hooks/useCart.test.ts` — cover calling `updateCartQuantity` from the API module and confirming the hook's `cart` state is refreshed
- [X] T024 [P] [US2] Add quantity control interaction tests to `test/frontend/components/Cart/CartPanel.test.tsx` — cover rendering a quantity input or stepper for each item and verifying `updateQuantity` is called with the correct `productId` and new value

### Implementation for User Story 2

- [X] T025 [US2] Implement `UpdateQuantity(int productId, int quantity)` in `src/backend/MockEcommerce.Api/Services/InMemoryCartService.cs` — find the existing cart item by `productId`, replace its `Quantity`, and return the updated `CartItem`; this method is wired for validation guards in US3
- [X] T026 [US2] Add `PUT /api/cart/{productId}` handler in `src/backend/MockEcommerce.Api/Endpoints/CartEndpoints.cs` — call `UpdateQuantity()`, build and return the updated `CartSummary` as `200 OK`
- [X] T027 [US2] Expose `updateQuantity(productId: number, quantity: number): Promise<void>` in `src/frontend/src/hooks/useCart.ts` — call `updateCartQuantity()` from the API module and call `refresh()` on success
- [X] T028 [US2] Add a quantity input or stepper control for each cart item in `src/frontend/src/components/Cart/CartPanel.tsx` — call `updateQuantity` on change; include `aria-label` attributes for accessibility

**Checkpoint**: User Stories 1 AND 2 are independently functional — shoppers can
add items, view the cart, and change quantities from within the cart panel.

---

## Phase 5: User Story 3 — Reject Invalid Quantity Changes (Priority: P3)

**Goal**: All invalid add and update attempts are rejected without changing cart
state. Shoppers receive a clear explanation of why their request failed.

**Independent Test**: Attempt to add a product already at quantity 5, attempt to
update an item not in the cart, and submit quantity 0 — verify the cart remains
unchanged and a clear error message is shown for each case.

### Tests for User Story 3 ⚠️

- [X] T029 [P] [US3] Add rejection tests to `test/backend/MockEcommerce.Api.Tests/Services/InMemoryCartServiceTests.cs` — cover: `Add` rejected when combined quantity would exceed 5; `UpdateQuantity` rejected when item is not in cart; `UpdateQuantity` rejected when quantity is 0 or negative
- [X] T030 [P] [US3] Add rejection tests to `test/backend/MockEcommerce.Api.Tests/Endpoints/CartEndpointTests.cs` — cover: `POST /api/cart` returns `400` with a validation problem when the quantity limit would be exceeded; `PUT /api/cart/{productId}` returns `400` on out-of-range quantity and `404` when the item is not in the cart
- [X] T031 [P] [US3] Add error state tests to `test/frontend/hooks/useCart.test.ts` and `test/frontend/components/Cart/CartPanel.test.tsx` — verify `error` is populated when the API returns a rejection and the error message is rendered in the panel

### Implementation for User Story 3

- [X] T032 [US3] Add quantity cap guard to `InMemoryCartService.Add()` in `src/backend/MockEcommerce.Api/Services/InMemoryCartService.cs` — return a validation failure result (not an exception) when the combined quantity after the add would exceed 5; the existing cart item must remain unchanged
- [X] T033 [US3] Add validation guards to `InMemoryCartService.UpdateQuantity()` in `src/backend/MockEcommerce.Api/Services/InMemoryCartService.cs` — return a not-found result when no cart item matches `productId`; return a validation failure result when `quantity` is outside 1–5 inclusive
- [X] T034 [US3] Map service result types to HTTP responses in `src/backend/MockEcommerce.Api/Endpoints/CartEndpoints.cs` for both `POST /api/cart` and `PUT /api/cart/{productId}` — validation failures → `400 ValidationProblem`; not-found results → `404` plain text; do not change the cart before returning
- [X] T035 [US3] Handle API error responses in `src/frontend/src/hooks/useCart.ts` — parse `400` and `404` response bodies and expose a typed `error: string | null` field; clear `error` on the next successful operation
- [X] T036 [US3] Render the `error` message in `src/frontend/src/components/Cart/CartPanel.tsx` — display the rejection reason near the affected quantity control; clear it when the next successful cart action refreshes state

**Checkpoint**: All three user stories are fully functional — invalid quantity
changes are rejected without mutating cart state and the shopper sees a clear
reason for each rejection.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Header badge accuracy, keyboard accessibility, and final end-to-end
test confirmation.

- [X] T037 Update `src/frontend/src/components/Header/Header.tsx` — ensure the cart icon badge renders `cart.itemCount` from the `CartSummary` and stays in sync after every cart mutation (add, update, or error)
- [X] T038 Audit `src/frontend/src/components/Cart/CartPanel.tsx` for keyboard accessibility — verify the panel can be opened and closed with keyboard only, all quantity controls have `aria-label` attributes, and focus returns to the header cart button on close
- [X] T039 Update `test/frontend/App.test.tsx` — cover that activating the header cart icon renders `CartPanel` and that triggering `onClose` removes it from the DOM
- [X] T040 Run `dotnet test test/backend/MockEcommerce.Api.Tests/MockEcommerce.Api.Tests.csproj` and `npm test` from the repository root and confirm all tests pass with no skipped assertions

---

## Dependencies

```
Phase 1 (Setup)
    └── Phase 2 (Foundational)
            ├── Phase 3 (US1 — Review Cart) 🎯 MVP
            │       └── Phase 4 (US2 — Adjust Quantities)
            │               └── Phase 5 (US3 — Reject Invalid Changes)
            │                       └── Phase 6 (Polish)
            │
            ├── T005, T006 (frontend types + API) unblock frontend track
            │   in parallel with the backend service/endpoint track
            │
            └── T003, T004 (C# model + interface) unblock backend track
```

## Parallel Execution per Story

### Story 1 (MVP sprint)

```
T007 InMemoryCartServiceTests (GetAll, Add)   ──┐
T008 CartEndpointTests (GET + POST success)   ──┤
T009 useCart.test.ts (initial load)           ──┤  write all tests in parallel
T010 CartPanel.test.tsx (display, empty)      ──┤  (all different files)
T011 Header.test.tsx (cart button)            ──┘
                          │ all tests written (red)
                          ▼
T012 InMemoryCartService.GetAll()
T013 InMemoryCartService.Add()
T014 GET /api/cart endpoint update
T015 POST /api/cart endpoint update
                          │ backend complete
T016 useCart.ts hook      ──┐ can start as soon as T006 (Phase 2) is done
T017 CartPanel.tsx        ──┤ can start as soon as T005 (Phase 2) is done
T019 Header.tsx update    ──┘ independent of T016 + T017
T018 Cart/index.ts (depends on T017)
T020 App.tsx (depends on T017 + T019)
```

### Story 2

```
T021 Service UpdateQuantity tests       ──┐
T022 Endpoint PUT tests                 ──┤  write all tests in parallel
T023 useCart.test.ts updateQuantity     ──┤  (all different files)
T024 CartPanel.test.tsx controls        ──┘
                          │
T025 InMemoryCartService.UpdateQuantity()
T026 PUT /api/cart/{productId} endpoint
T027 useCart.ts updateQuantity function
T028 CartPanel.tsx quantity controls
```

### Story 3

```
T029 Service rejection tests            ──┐
T030 Endpoint rejection tests           ──┤  write all tests in parallel
T031 Frontend error state tests         ──┘
                          │
T032 Service Add() quantity cap guard
T033 Service UpdateQuantity() validation guards
T034 Endpoint error response mapping
T035 useCart.ts error handling
T036 CartPanel.tsx error display
```

## Implementation Strategy

**MVP Scope (Story 1 only)**: Delivers a fully working cart review experience —
shoppers can add products and open the cart to see all items, quantities, unit
prices, line totals, and the cart subtotal. This phase is independently
shippable and satisfies SC-001.

**Increment 1 (Story 2 on top of MVP)**: Adds in-cart quantity adjustment.
Depends on the CartPanel from US1 and the PUT endpoint contract from Phase 2.
Satisfies SC-002.

**Increment 2 (Story 3 on top of Increment 1)**: Completes the loop with
structured rejection feedback for all invalid inputs. Depends on the service
result types scaffolded during US1 and US2. Satisfies SC-003, SC-004, SC-005.

**Full Feature (Phase 6 Polish)**: Ensures badge accuracy, keyboard navigation,
and final automated confirmation across all layers.
