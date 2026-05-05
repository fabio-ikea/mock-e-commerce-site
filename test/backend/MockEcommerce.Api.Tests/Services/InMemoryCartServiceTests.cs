using MockEcommerce.Api.Models;
using MockEcommerce.Api.Services;

namespace MockEcommerce.Api.Tests.Services;

public class InMemoryCartServiceTests
{
    private readonly InMemoryCartService _service;

    // Product 1 from MockProductService: Wireless Headphones, $79.99
    private static CartItem HeadphonesItem(int quantity = 1) => new()
    {
        ProductId = 1,
        ProductName = "Wireless Headphones",
        UnitPrice = 79.99m,
        Quantity = quantity
    };

    // Product 2 from MockProductService: Running Shoes, $59.99
    private static CartItem ShoesItem(int quantity = 1) => new()
    {
        ProductId = 2,
        ProductName = "Running Shoes",
        UnitPrice = 59.99m,
        Quantity = quantity
    };

    public InMemoryCartServiceTests()
    {
        _service = new InMemoryCartService();
    }

    // ─── US1: Review Cart ────────────────────────────────────────────────────

    [Fact]
    public void GetAll_OnFreshService_ReturnsEmptyList()
    {
        var result = _service.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public void Add_NewItem_ReturnsSuccessWithIsNewTrue()
    {
        var result = _service.Add(HeadphonesItem());

        Assert.Equal(CartResultStatus.Success, result.Status);
        Assert.True(result.IsNew);
        Assert.NotNull(result.Item);
        Assert.Equal(1, result.Item!.ProductId);
    }

    [Fact]
    public void Add_NewItem_PersistsItemInCart()
    {
        _service.Add(HeadphonesItem());

        var items = _service.GetAll().ToList();

        Assert.Single(items);
        Assert.Equal(1, items[0].ProductId);
        Assert.Equal("Wireless Headphones", items[0].ProductName);
        Assert.Equal(79.99m, items[0].UnitPrice);
        Assert.Equal(1, items[0].Quantity);
    }

    [Fact]
    public void Add_ExistingItem_IncrementsQuantityAndReturnsIsNewFalse()
    {
        _service.Add(HeadphonesItem(2));

        var result = _service.Add(HeadphonesItem(1));

        Assert.Equal(CartResultStatus.Success, result.Status);
        Assert.False(result.IsNew);
        Assert.Equal(3, result.Item!.Quantity);
    }

    [Fact]
    public void Add_ExistingItem_IncrementsQuantityInCart()
    {
        _service.Add(HeadphonesItem(2));
        _service.Add(HeadphonesItem(1));

        var items = _service.GetAll().ToList();

        Assert.Single(items);
        Assert.Equal(3, items[0].Quantity);
    }

    // ─── US2: Adjust Quantities ───────────────────────────────────────────────

    [Fact]
    public void UpdateQuantity_WhenItemExists_ReturnsSuccessWithUpdatedQuantity()
    {
        _service.Add(HeadphonesItem(2));

        var result = _service.UpdateQuantity(1, 4);

        Assert.Equal(CartResultStatus.Success, result.Status);
        Assert.NotNull(result.Item);
        Assert.Equal(4, result.Item!.Quantity);
    }

    [Fact]
    public void UpdateQuantity_ToOne_UpdatesCartItem()
    {
        _service.Add(HeadphonesItem(3));

        _service.UpdateQuantity(1, 1);

        var item = _service.GetByProductId(1);
        Assert.NotNull(item);
        Assert.Equal(1, item!.Quantity);
    }

    [Fact]
    public void UpdateQuantity_ToFive_UpdatesCartItem()
    {
        _service.Add(HeadphonesItem(1));

        _service.UpdateQuantity(1, 5);

        var item = _service.GetByProductId(1);
        Assert.NotNull(item);
        Assert.Equal(5, item!.Quantity);
    }

    // ─── US3: Reject Invalid Changes ────────────────────────────────────────

    [Fact]
    public void Add_WhenCombinedQuantityExceedsFive_ReturnsValidationError()
    {
        _service.Add(HeadphonesItem(4));

        var result = _service.Add(HeadphonesItem(2));

        Assert.Equal(CartResultStatus.ValidationError, result.Status);
        Assert.NotNull(result.Message);
    }

    [Fact]
    public void Add_WhenCombinedQuantityExceedsFive_DoesNotMutateCart()
    {
        _service.Add(HeadphonesItem(4));
        _service.Add(HeadphonesItem(2)); // rejected

        var item = _service.GetByProductId(1);
        Assert.Equal(4, item!.Quantity);
    }

    [Fact]
    public void Add_WhenNewItemQuantityExceedsFive_ReturnsValidationError()
    {
        var result = _service.Add(HeadphonesItem(6));

        Assert.Equal(CartResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void UpdateQuantity_WhenItemNotInCart_ReturnsNotFound()
    {
        var result = _service.UpdateQuantity(99, 3);

        Assert.Equal(CartResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void UpdateQuantity_WithZeroQuantity_ReturnsValidationError()
    {
        _service.Add(HeadphonesItem());

        var result = _service.UpdateQuantity(1, 0);

        Assert.Equal(CartResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void UpdateQuantity_WithNegativeQuantity_ReturnsValidationError()
    {
        _service.Add(HeadphonesItem());

        var result = _service.UpdateQuantity(1, -1);

        Assert.Equal(CartResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void UpdateQuantity_WithQuantityAboveFive_ReturnsValidationError()
    {
        _service.Add(HeadphonesItem());

        var result = _service.UpdateQuantity(1, 6);

        Assert.Equal(CartResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void UpdateQuantity_WhenValidationFails_DoesNotMutateCart()
    {
        _service.Add(HeadphonesItem(3));
        _service.UpdateQuantity(1, 0); // rejected

        var item = _service.GetByProductId(1);
        Assert.Equal(3, item!.Quantity);
    }
}
