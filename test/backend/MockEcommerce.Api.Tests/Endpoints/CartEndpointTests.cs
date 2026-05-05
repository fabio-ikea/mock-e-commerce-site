using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MockEcommerce.Api.Endpoints;
using MockEcommerce.Api.Models;

namespace MockEcommerce.Api.Tests.Endpoints;

public class CartEndpointTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public CartEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Reset cart state before each test for isolation
        await _client.DeleteAsync("/api/cart/");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ─── US1: Review Cart ────────────────────────────────────────────────────

    [Fact]
    public async Task GetCart_WhenEmpty_ReturnsOkWithEmptyCartSummary()
    {
        var response = await _client.GetAsync("/api/cart/");

        response.EnsureSuccessStatusCode();
        var summary = await response.Content.ReadFromJsonAsync<CartSummary>();
        Assert.NotNull(summary);
        Assert.Empty(summary!.Items);
        Assert.Equal(0, summary.ItemCount);
        Assert.Equal(0m, summary.Subtotal);
    }

    [Fact]
    public async Task AddToCart_WithValidRequest_ReturnsCreatedWithCartSummary()
    {
        var request = new AddToCartRequest(ProductId: 1, Quantity: 2);

        var response = await _client.PostAsJsonAsync("/api/cart/", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var summary = await response.Content.ReadFromJsonAsync<CartSummary>();
        Assert.NotNull(summary);
        Assert.Single(summary!.Items);
        Assert.Equal(1, summary.Items[0].ProductId);
        Assert.Equal(2, summary.Items[0].Quantity);
        Assert.Equal(2, summary.ItemCount);
        Assert.True(summary.Subtotal > 0);
    }

    [Fact]
    public async Task AddToCart_ExistingItem_ReturnsOkWithIncrementedQuantity()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 1));

        var response = await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 2));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = await response.Content.ReadFromJsonAsync<CartSummary>();
        Assert.NotNull(summary);
        Assert.Single(summary!.Items);
        Assert.Equal(3, summary.Items[0].Quantity);
        Assert.Equal(3, summary.ItemCount);
    }

    [Fact]
    public async Task GetCart_AfterAddingItems_ReturnsPopulatedCartSummary()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 1));
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(2, 2));

        var response = await _client.GetAsync("/api/cart/");

        response.EnsureSuccessStatusCode();
        var summary = await response.Content.ReadFromJsonAsync<CartSummary>();
        Assert.NotNull(summary);
        Assert.Equal(2, summary!.Items.Count);
        Assert.Equal(3, summary.ItemCount);
    }

    // ─── US2: Adjust Quantities ───────────────────────────────────────────────

    [Fact]
    public async Task UpdateCartQuantity_WithValidRequest_ReturnsOkWithUpdatedCartSummary()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 1));

        var response = await _client.PutAsJsonAsync("/api/cart/1",
            new UpdateCartQuantityRequest(Quantity: 3));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = await response.Content.ReadFromJsonAsync<CartSummary>();
        Assert.NotNull(summary);
        Assert.Single(summary!.Items);
        Assert.Equal(3, summary.Items[0].Quantity);
        Assert.Equal(3, summary.ItemCount);
    }

    // ─── US3: Reject Invalid Changes ────────────────────────────────────────

    [Fact]
    public async Task AddToCart_WhenQuantityLimitExceeded_ReturnsBadRequest()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 4));

        var response = await _client.PostAsJsonAsync("/api/cart/",
            new AddToCartRequest(1, 2));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddToCart_WhenQuantityLimitExceeded_DoesNotChangeCart()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 4));
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 2)); // rejected

        var summary = await _client.GetFromJsonAsync<CartSummary>("/api/cart/");
        Assert.NotNull(summary);
        Assert.Equal(4, summary!.Items[0].Quantity);
    }

    [Fact]
    public async Task AddToCart_WithProductNotFound_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/cart/",
            new AddToCartRequest(ProductId: 9999, Quantity: 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartQuantity_WithOutOfRangeQuantity_ReturnsBadRequest()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 1));

        var response = await _client.PutAsJsonAsync("/api/cart/1",
            new UpdateCartQuantityRequest(Quantity: 6));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartQuantity_WithZeroQuantity_ReturnsBadRequest()
    {
        await _client.PostAsJsonAsync("/api/cart/", new AddToCartRequest(1, 1));

        var response = await _client.PutAsJsonAsync("/api/cart/1",
            new UpdateCartQuantityRequest(Quantity: 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartQuantity_WhenItemNotInCart_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/cart/99",
            new UpdateCartQuantityRequest(Quantity: 3));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
