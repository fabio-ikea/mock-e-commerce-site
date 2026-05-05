namespace MockEcommerce.Api.Models;

/// <summary>Aggregate cart state returned to the frontend for review.</summary>
public class CartSummary
{
    /// <summary>All items currently in the cart.</summary>
    public IReadOnlyList<CartItem> Items { get; init; } = [];

    /// <summary>Sum of all cart item quantities.</summary>
    public int ItemCount { get; init; }

    /// <summary>Sum of all line totals.</summary>
    public decimal Subtotal { get; init; }
}
