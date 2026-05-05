using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Services;

/// <summary>
/// Defines operations for managing shopping cart items.
/// </summary>
public interface ICartService
{
    /// <summary>Returns all items currently in the cart.</summary>
    IEnumerable<CartItem> GetAll();

    /// <summary>
    /// Adds a product to the cart or increments its quantity if already present.
    /// </summary>
    /// <param name="item">The cart item to add (with product details pre-resolved).</param>
    /// <returns>
    /// A <see cref="CartServiceResult"/> indicating success, validation failure (quantity cap),
    /// or not-found.
    /// </returns>
    CartServiceResult Add(CartItem item);

    /// <summary>
    /// Replaces the quantity of an existing cart item.
    /// </summary>
    /// <param name="productId">The product whose quantity to replace.</param>
    /// <param name="quantity">The new quantity (must be 1–5 inclusive).</param>
    /// <returns>
    /// A <see cref="CartServiceResult"/> indicating success, validation failure,
    /// or not-found when the product is not currently in the cart.
    /// </returns>
    CartServiceResult UpdateQuantity(int productId, int quantity);

    /// <summary>
    /// Finds an existing cart item by product ID.
    /// </summary>
    /// <param name="productId">The product ID to look up.</param>
    /// <returns>The matching cart item, or <c>null</c> if not found.</returns>
    CartItem? GetByProductId(int productId);

    /// <summary>
    /// Removes a cart item by product ID.
    /// </summary>
    /// <param name="productId">The product ID to remove.</param>
    /// <returns><c>true</c> if the item was found and removed; otherwise <c>false</c>.</returns>
    bool Remove(int productId);

    /// <summary>Removes all items from the cart.</summary>
    void Clear();
}
