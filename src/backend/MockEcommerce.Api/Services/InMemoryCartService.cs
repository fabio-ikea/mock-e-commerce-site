using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Services;

/// <summary>
/// Thread-safe in-memory cart storage. Registered as Singleton for demo purposes;
/// all users share a single cart. Replace with a per-user scoped implementation
/// when authentication is added.
/// </summary>
public class InMemoryCartService : ICartService
{
    private readonly List<CartItem> _cart = [];
    private readonly Lock _lock = new();

    private const int MaxQuantityPerProduct = 5;

    /// <inheritdoc />
    public IEnumerable<CartItem> GetAll()
    {
        lock (_lock)
        {
            return _cart.ToList();
        }
    }

    /// <inheritdoc />
    public CartItem? GetByProductId(int productId)
    {
        lock (_lock)
        {
            return _cart.FirstOrDefault(i => i.ProductId == productId);
        }
    }

    /// <inheritdoc />
    public CartServiceResult Add(CartItem item)
    {
        lock (_lock)
        {
            var existing = _cart.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing is not null)
            {
                int newQty = existing.Quantity + item.Quantity;
                if (newQty > MaxQuantityPerProduct)
                    return new CartServiceResult(
                        CartResultStatus.ValidationError,
                        Message: $"Cannot add {item.Quantity} unit(s): quantity would exceed the maximum of {MaxQuantityPerProduct} per product.");

                existing.Quantity = newQty;
                return new CartServiceResult(CartResultStatus.Success, existing, IsNew: false);
            }
            else
            {
                if (item.Quantity > MaxQuantityPerProduct)
                    return new CartServiceResult(
                        CartResultStatus.ValidationError,
                        Message: $"Quantity must be between 1 and {MaxQuantityPerProduct}.");

                _cart.Add(item);
                return new CartServiceResult(CartResultStatus.Success, item, IsNew: true);
            }
        }
    }

    /// <inheritdoc />
    public CartServiceResult UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 1 || quantity > MaxQuantityPerProduct)
            return new CartServiceResult(
                CartResultStatus.ValidationError,
                Message: $"Quantity must be between 1 and {MaxQuantityPerProduct}.");

        lock (_lock)
        {
            var existing = _cart.FirstOrDefault(i => i.ProductId == productId);
            if (existing is null)
                return new CartServiceResult(
                    CartResultStatus.NotFound,
                    Message: "The specified product is not in the cart.");

            existing.Quantity = quantity;
            return new CartServiceResult(CartResultStatus.Success, existing);
        }
    }

    /// <inheritdoc />
    public bool Remove(int productId)
    {
        lock (_lock)
        {
            var item = _cart.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return false;
            _cart.Remove(item);
            return true;
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (_lock)
        {
            _cart.Clear();
        }
    }
}

