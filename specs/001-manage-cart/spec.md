# Feature Specification: Manage Cart

**Feature Branch**: `[001-manage-cart]`  
**Created**: 2026-05-05  
**Status**: Draft  
**Input**: User description: "Users should be able to view their cart, see what they're paying, and manage their selections before checkout. The cart should be accessible from the existing cart icon in the header. Each product has a maximum purchase quantity of 5 - attempts to add more than 5 of any single item should be rejected. In addition to adding items, users should be able to update the quantity of an item already in their cart via a PUT endpoint."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Review cart contents (Priority: P1)

As a shopper, I want to open my cart from the existing header cart icon and review the items I selected, so that I can confirm what I am about to purchase before checkout.

**Why this priority**: Users cannot confidently continue to checkout unless they can inspect their selected items and pricing.

**Independent Test**: Add one or more products to the cart, open the cart from the header icon, and verify that item details, quantities, line totals, and the cart total are shown correctly.

**Acceptance Scenarios**:

1. **Given** a shopper has items in the cart, **When** they select the existing cart icon in the header, **Then** the cart view opens and shows each selected item with its name, quantity, unit price, line total, and overall cart total.
2. **Given** a shopper has no items in the cart, **When** they open the cart, **Then** they see an empty-cart state that explains there are no selected items yet.

---

### User Story 2 - Adjust selected quantities (Priority: P2)

As a shopper, I want to change the quantity of an item already in my cart, so that I can adjust my order before checkout without re-adding the item from the product listing.

**Why this priority**: Quantity management is central to cart usefulness and directly affects what the shopper will pay.

**Independent Test**: Add an item to the cart, change its quantity while it remains in the cart, and verify that the new quantity and pricing update immediately and accurately.

**Acceptance Scenarios**:

1. **Given** a shopper has an item in the cart with a quantity below the maximum, **When** they update the quantity to another valid amount between 1 and 5, **Then** the cart stores the new quantity and refreshes all affected totals.
2. **Given** a shopper has an item in the cart, **When** they increase its quantity through another add action or a cart quantity update, **Then** the combined quantity must never exceed 5 for that product.

---

### User Story 3 - Reject invalid quantity changes (Priority: P3)

As a shopper, I want invalid quantity changes to be rejected clearly, so that I understand why my cart did not change and can correct the issue.

**Why this priority**: Clear rejection handling protects the cart rules and avoids pricing confusion.

**Independent Test**: Attempt to set or add a quantity above the allowed maximum, attempt to update an item that is not currently in the cart, and verify that the cart remains unchanged while the user receives a clear explanation.

**Acceptance Scenarios**:

1. **Given** a shopper already has 5 units of a product in the cart, **When** they attempt to add more of that product or set its quantity above 5, **Then** the request is rejected and the existing cart quantity remains unchanged.
2. **Given** a shopper attempts to update a product that is not currently in the cart, **When** the update is submitted, **Then** the request is rejected and the cart remains unchanged.
3. **Given** a shopper submits an invalid quantity such as 0, a negative number, or a non-numeric value, **When** the system receives the request, **Then** the request is rejected and the shopper receives a clear error message.

### Edge Cases

- If an add action would cause a product quantity to exceed 5, the entire add attempt is rejected and the existing quantity remains unchanged.
- If a direct quantity update requests more than 5 for a product, the update is rejected and no partial adjustment is applied.
- If a direct quantity update targets a product that is not already in the cart, the update is rejected.
- If a shopper opens the cart while it is empty, the view shows a clear empty state instead of blank content.
- If a product referenced in the cart can no longer be found in the active catalog, the cart operation is rejected with a clear message and no silent pricing change occurs.
- If pricing information changes while a shopper is reviewing the cart, the cart must display the latest available totals the next time it is loaded or refreshed.
- The cart view must remain usable with keyboard-only navigation and readable labels for controls and totals.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST allow shoppers to open the cart from the existing header cart icon.
- **FR-002**: The system MUST display all selected cart items with enough detail for shoppers to review what they are paying, including per-item quantity, per-item price, per-item extended total, and overall cart total.
- **FR-003**: The system MUST provide an empty-cart state when no items have been selected.
- **FR-004**: The system MUST allow shoppers to update the quantity of an item that is already in their cart.
- **FR-005**: The system MUST reject any add or update action that would result in more than 5 units of a single product in the cart.
- **FR-006**: The system MUST reject quantity updates for products that are not already present in the cart.
- **FR-007**: The system MUST reject invalid quantity inputs, including values below 1 and non-numeric values, without changing the cart.
- **FR-008**: The system MUST leave the cart unchanged whenever a quantity change is rejected.
- **FR-009**: The system MUST provide a clear error message when a quantity change is rejected so the shopper understands how to correct the request.
- **FR-010**: The system MUST define expected loading, success, empty, and error states for the cart experience.
- **FR-011**: The cart update operation MUST support changing the quantity of an existing cart item without requiring the shopper to add the product again from the catalog.
- **FR-012**: The cart experience MUST remain accessible through keyboard navigation and clearly labeled controls.
- **FR-013**: API contract changes for this feature include a cart quantity update operation for an existing cart item and cart responses that expose the values needed for item-level and cart-level pricing review.

### Key Entities *(include if feature involves data)*

- **Cart**: A shopper’s current set of selected products before checkout, including its items and overall total.
- **Cart Item**: A selected product within the cart, including product identity, quantity, unit price, and extended total.
- **Quantity Update Request**: A shopper-submitted change that sets the desired quantity for an existing cart item.
- **Cart Validation Error**: A rejected cart action that explains why the cart did not change, such as exceeding the quantity limit or targeting an item not in the cart.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of shoppers can open the cart from the header icon and see the current contents of a non-empty cart during acceptance testing.
- **SC-002**: 100% of valid quantity changes between 1 and 5 update the displayed totals correctly during acceptance testing.
- **SC-003**: 100% of attempts to exceed 5 units of a single product are rejected without changing the cart.
- **SC-004**: 100% of attempts to update an item that is not already in the cart are rejected with a clear explanation.
- **SC-005**: Feature acceptance is verified through automated coverage of cart display, valid quantity updates, invalid quantity rejection, and empty-cart behavior.

## Assumptions

- Taxes, shipping, discounts, and checkout payment calculations are out of scope; “see what they’re paying” means item pricing totals and overall cart subtotal for selected products.
- This feature adds quantity review and update behavior only; explicit item removal is not introduced unless it already exists elsewhere.
- A valid cart quantity is an integer between 1 and 5 inclusive for each product.
- The existing header cart icon remains the primary entry point to the cart experience.
- The cart belongs to a single shopper session and does not require multi-user collaboration behavior.
