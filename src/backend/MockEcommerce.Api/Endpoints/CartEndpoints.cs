using Microsoft.AspNetCore.Http.HttpResults;
using MockEcommerce.Api.Models;
using MockEcommerce.Api.Services;

namespace MockEcommerce.Api.Endpoints;

/// <summary>
/// Maps shopping cart endpoints under <c>/api/cart</c>.
/// </summary>
public static class CartEndpoints
{
    /// <summary>Registers cart-related routes on the given endpoint route builder.</summary>
    public static void MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/cart")
            .WithTags("Cart");

        group.MapGet("/", GetCart)
            .WithName("GetCart")
            .WithSummary("Returns the current cart summary.");

        group.MapPost("/", AddToCart)
            .WithName("AddToCart")
            .WithSummary("Adds a product to the cart or increments quantity if already present.");

        group.MapPut("/{productId:int}", UpdateCartQuantity)
            .WithName("UpdateCartQuantity")
            .WithSummary("Replaces the quantity of an existing cart item.");

        group.MapDelete("/{productId:int}", RemoveFromCart)
            .WithName("RemoveFromCart")
            .WithSummary("Removes a single product from the cart by its product ID.");

        group.MapDelete("/", ClearCart)
            .WithName("ClearCart")
            .WithSummary("Removes all items from the cart.");
    }

    /// <summary>Returns the current cart as a <see cref="CartSummary"/>.</summary>
    internal static Ok<CartSummary> GetCart(ICartService cartService)
    {
        return TypedResults.Ok(BuildCartSummary(cartService));
    }

    /// <summary>Adds a product to the cart or increments quantity if already present.</summary>
    internal static Results<Created<CartSummary>, Ok<CartSummary>, NotFound<string>, ValidationProblem> AddToCart(
        AddToCartRequest request,
        IProductService productService,
        ICartService cartService)
    {
        if (request.Quantity < 1)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "quantity", ["Quantity must be at least 1."] }
            });
        }

        var product = productService.GetById(request.ProductId);
        if (product is null)
            return TypedResults.NotFound($"Product {request.ProductId} was not found.");

        var item = new CartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            UnitPrice = product.Price,
            Quantity = request.Quantity
        };

        var result = cartService.Add(item);

        if (result.Status == CartResultStatus.ValidationError)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "quantity", [result.Message ?? "Invalid quantity."] }
            });
        }

        var summary = BuildCartSummary(cartService);
        return result.IsNew
            ? TypedResults.Created("/api/cart", summary)
            : TypedResults.Ok(summary);
    }

    /// <summary>Replaces the quantity of an existing cart item.</summary>
    internal static Results<Ok<CartSummary>, NotFound<string>, ValidationProblem> UpdateCartQuantity(
        int productId,
        UpdateCartQuantityRequest request,
        ICartService cartService)
    {
        var result = cartService.UpdateQuantity(productId, request.Quantity);

        return result.Status switch
        {
            CartResultStatus.NotFound =>
                TypedResults.NotFound(result.Message ?? "Cart item not found."),
            CartResultStatus.ValidationError =>
                TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "quantity", [result.Message ?? "Invalid quantity."] }
                }),
            _ => TypedResults.Ok(BuildCartSummary(cartService))
        };
    }

    /// <summary>Removes a single product from the cart by its product ID.</summary>
    internal static Results<NoContent, NotFound> RemoveFromCart(int productId, ICartService cartService)
    {
        return cartService.Remove(productId)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }

    /// <summary>Removes all items from the cart.</summary>
    internal static NoContent ClearCart(ICartService cartService)
    {
        cartService.Clear();
        return TypedResults.NoContent();
    }

    private static CartSummary BuildCartSummary(ICartService cartService)
    {
        var items = cartService.GetAll().ToList();
        return new CartSummary
        {
            Items = items,
            ItemCount = items.Sum(i => i.Quantity),
            Subtotal = items.Sum(i => i.TotalPrice)
        };
    }
}

/// <summary>Request body for adding a product to the cart.</summary>
public record AddToCartRequest(int ProductId, int Quantity);

