namespace MockEcommerce.Api.Models;

/// <summary>Outcome of a cart mutation operation.</summary>
public enum CartResultStatus
{
    /// <summary>Operation succeeded.</summary>
    Success,

    /// <summary>A business rule was violated (e.g. quantity out of range).</summary>
    ValidationError,

    /// <summary>The target item was not found (product or cart item).</summary>
    NotFound,
}

/// <summary>Result returned by <see cref="MockEcommerce.Api.Services.ICartService"/> mutation methods.</summary>
public record CartServiceResult(
    CartResultStatus Status,
    CartItem? Item = null,
    string? Message = null,
    bool IsNew = false);
