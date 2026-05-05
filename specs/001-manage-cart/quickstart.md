# Quickstart: Manage Cart

## Prerequisites

- .NET 10 SDK
- Node.js compatible with the frontend workspace
- Dependencies restored from the repository root and `src/frontend`

## Run the application

### Backend

From the repository root:

- `dotnet run --project src/backend/MockEcommerce.Api`

### Frontend

From the repository root:

- `npm install`
- `npm run dev --workspace src/frontend`

Open the frontend URL shown by Vite and keep the backend running on its default local port.

## Manual validation scenarios

### 1. Review the cart from the header icon

1. Load the storefront.
2. Add one product.
3. Open the cart using the existing header cart button.
4. Confirm the cart shows item name, quantity, unit price, line total, and subtotal.

### 2. Update quantity for an existing cart item

1. Add one product to the cart.
2. Open the cart.
3. Change the quantity to `3`.
4. Confirm the cart updates the item quantity, line total, subtotal, and cart count.

### 3. Reject quantities above the limit

1. Add a product until it reaches quantity `5`.
2. Attempt to add the same product again.
3. Confirm the request is rejected, the cart remains unchanged, and a clear error is shown.
4. Attempt to set the quantity directly to `6` and confirm the same rejection behavior.

### 4. Reject updates for missing items

1. Ensure a product is not present in the cart.
2. Attempt a quantity update for that product.
3. Confirm the request is rejected and the cart contents do not change.

### 5. Verify empty-cart behavior

1. Clear the cart or start with a fresh empty cart.
2. Open the cart from the header icon.
3. Confirm the UI shows the empty-cart state rather than a blank panel.

## Automated checks

From the repository root:

- `npm test`
- `dotnet test test/backend/MockEcommerce.Api.Tests/MockEcommerce.Api.Tests.csproj`

Use these checks after implementation to validate frontend and backend coverage for cart review, quantity updates, and rejection paths.
