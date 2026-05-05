namespace MockEcommerce.Api.Models;

/// <summary>Request body for replacing the quantity of an existing cart item.</summary>
public record UpdateCartQuantityRequest(int Quantity);
