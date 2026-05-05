# Data Model: Manage Cart

## Entities

### CartSummary
Represents the cart state returned to the frontend for review.

| Field | Type | Description | Rules |
|-------|------|-------------|-------|
| `items` | `CartItem[]` | Current cart line items | May be empty |
| `itemCount` | integer | Sum of all cart item quantities | Derived from `items`; must be `>= 0` |
| `subtotal` | decimal | Sum of all line totals | Derived from `items`; must be `>= 0` |

### CartItem
Represents one selected product in the cart.

| Field | Type | Description | Rules |
|-------|------|-------------|-------|
| `productId` | integer | Catalog product identifier | Required; must map to an existing product when created |
| `productName` | string | Display name snapshot | Required; non-empty |
| `unitPrice` | decimal | Product price snapshot | Required; must be `>= 0` |
| `quantity` | integer | Units currently selected | Required; integer from `1` to `5` inclusive |
| `totalPrice` | decimal | Extended line total | Derived as `unitPrice * quantity` |

### AddToCartRequest
Represents a request to add units of a product to the cart.

| Field | Type | Description | Rules |
|-------|------|-------------|-------|
| `productId` | integer | Product to add | Required; must exist in catalog |
| `quantity` | integer | Units to add | Required; integer `>= 1`; combined quantity after add must be `<= 5` |

### UpdateCartQuantityRequest
Represents a request to replace the quantity of an existing cart item.

| Field | Type | Description | Rules |
|-------|------|-------------|-------|
| `quantity` | integer | New desired quantity | Required; integer from `1` to `5` inclusive |

### CartValidationError
Represents a rejected cart mutation.

| Field | Type | Description | Rules |
|-------|------|-------------|-------|
| `type` | string | Error category | `validation` or `not-found` |
| `message` | string | Human-readable explanation | Required |
| `fieldErrors` | map | Per-field validation details when applicable | Present for validation failures |

## Relationships

- `CartSummary` contains zero or more `CartItem` records.
- `CartItem.productId` references a `Product.id` in the catalog at the time the item is added.
- `AddToCartRequest` and `UpdateCartQuantityRequest` can each produce an updated `CartSummary` on success.
- `CartValidationError` can be returned by add and update operations when business rules are violated.

## Validation Rules

- Cart quantities are integers only.
- Valid cart quantity range is `1..5` inclusive per product.
- Add operations reject requests that would push an existing item above `5`.
- Update operations reject requests for products not already present in the cart.
- Rejected mutations do not change `CartSummary.items`, `itemCount`, or `subtotal`.
- Empty carts return an empty `items` collection with `itemCount = 0` and `subtotal = 0`.

## State Transitions

- **Empty cart -> Populated cart**: First successful add creates the first `CartItem`.
- **Populated cart -> Updated cart**: Successful add increments an existing quantity or successful PUT replaces an existing quantity.
- **Populated cart -> Same populated cart**: Invalid add or update is rejected and no cart state changes.
- **Populated cart -> Empty cart**: Existing delete or clear behavior removes all items.
