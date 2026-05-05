# Research: Manage Cart

## Decision: Use the backend cart as the source of truth for cart totals and counts
- **Rationale**: The existing application already posts cart mutations to the backend. Keeping the backend as the authoritative source avoids duplicated pricing logic in the frontend and ensures the cart icon count, line totals, and subtotal stay aligned after add and update actions.
- **Alternatives considered**:
  - **Frontend-only cart state**: Rejected because it would duplicate cart pricing and validation logic and drift from the API contract.
  - **Hybrid optimistic state without refresh**: Rejected because the quantity cap and error handling become harder to keep consistent.

## Decision: `PUT /api/cart/{productId}` replaces the quantity of an existing cart item
- **Rationale**: The specification calls for updating the quantity of an item already in the cart. Treating PUT as a full replacement of the quantity makes the API predictable and resolves the ambiguity between “increment” and “set quantity.”
- **Alternatives considered**:
  - **Increment-on-PUT semantics**: Rejected because it duplicates POST behavior and makes quantity management harder to reason about.
  - **Upsert semantics**: Rejected because the spec explicitly limits PUT to items already in the cart.

## Decision: Quantity validation is enforced identically on add and update
- **Rationale**: The cart rule is a business invariant, not a UI rule. Both POST and PUT reject quantities that would leave a product below 1 or above 5, and rejected operations leave the cart unchanged.
- **Alternatives considered**:
  - **Allow partial clamping to 5**: Rejected because silent correction hides invalid user intent.
  - **Allow POST to exceed 5 while PUT cannot**: Rejected because inconsistent rules would confuse users and complicate tests.

## Decision: Validation failures return structured validation errors; missing resources return not-found responses
- **Rationale**: Invalid quantity inputs and quantity-limit breaches are field-level problems and should return a validation payload the frontend can map to helpful messages. Missing catalog products or missing cart items are absence errors and should return a not-found response.
- **Alternatives considered**:
  - **Single generic 400 for all failures**: Rejected because the frontend cannot distinguish bad input from missing items.
  - **Success responses with warning messages**: Rejected because invalid cart changes must not appear to succeed.

## Decision: The cart UI will open from the existing header icon as a dedicated review surface
- **Rationale**: The feature requires cart access from the current header entry point. A dedicated cart panel or drawer keeps shoppers in context, supports quick review, and surfaces loading, empty, success, and error states without routing away from the catalog.
- **Alternatives considered**:
  - **Separate full-page cart route**: Rejected because it adds navigation overhead for a small review flow.
  - **Inline cart embedded in the product grid**: Rejected because it weakens discoverability and complicates layout.

## Decision: Cart responses should include aggregate values needed for review
- **Rationale**: The shopper needs to see what they are paying before checkout. Returning cart items together with aggregate count and subtotal gives the frontend everything needed to render the cart and header state without recalculating business data in multiple places.
- **Alternatives considered**:
  - **Return item rows only and derive subtotal entirely in the frontend**: Rejected because it duplicates money calculations.
  - **Return only the changed item after POST/PUT**: Rejected because the cart view and header count still need synchronized aggregate values.
