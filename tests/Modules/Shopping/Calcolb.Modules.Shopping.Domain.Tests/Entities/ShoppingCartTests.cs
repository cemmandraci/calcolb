using Calcolb.Modules.Shopping.Domain.Entities;
using Calcolb.Modules.Shopping.Domain.Events;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Shopping.Domain.Tests.Entities;

public class ShoppingCartTests
{
    private static Product ActiveProduct(string name = "Süt") =>
        Product.Create(name, Guid.NewGuid(), "111", "litre", "Migros", true);

    private static Product InactiveProduct() =>
        Product.Create("Bayat Ekmek", Guid.NewGuid(), "222", "adet", "Bim", false);

    // --- Create ---

    [Fact]
    public void Create_WhenValidEventId_ReturnsShoppingCart()
    {
        var eventId = Guid.NewGuid();

        var cart = ShoppingCart.Create(eventId);

        cart.ShouldNotBeNull();
        cart.Id.ShouldNotBe(Guid.Empty);
        cart.EventId.ShouldBe(eventId);
        cart.CartItems.ShouldBeEmpty();
        cart.TotalCost.ShouldBe(0m);
    }

    [Fact]
    public void Create_WhenEventIdIsEmpty_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => ShoppingCart.Create(Guid.Empty));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var cart1 = ShoppingCart.Create(Guid.NewGuid());
        var cart2 = ShoppingCart.Create(Guid.NewGuid());

        cart1.Id.ShouldNotBe(cart2.Id);
    }

    // --- AddItem ---

    [Fact]
    public void AddItem_WhenActiveProduct_AddsCartItem()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = ActiveProduct();

        var cartItem = cart.AddItem(product, 2m, 30m);

        cart.CartItems.Count.ShouldBe(1);
        cartItem.ProductId.ShouldBe(product.Id);
        cartItem.Quantity.ShouldBe(2m);
        cartItem.UnitPrice.ShouldBe(30m);
        cartItem.TotalPrice.ShouldBe(60m);
    }

    [Fact]
    public void AddItem_WhenInactiveProduct_ThrowsDomainException()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = InactiveProduct();

        Should.Throw<DomainException>(() => cart.AddItem(product, 1m, 10m))
            .Message.ShouldContain("aktif değil");
    }

    [Fact]
    public void AddItem_SetsUnitFromProduct()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = ActiveProduct();

        var cartItem = cart.AddItem(product, 1m, 20m);

        cartItem.Unit.ShouldBe(product.Unit);
        cartItem.MarketType.ShouldBe(product.MarketType);
    }

    [Fact]
    public void AddItem_MultipleItems_AddsAll()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var p1 = ActiveProduct("Süt");
        var p2 = ActiveProduct("Ekmek");

        cart.AddItem(p1, 1m, 20m);
        cart.AddItem(p2, 2m, 15m);

        cart.CartItems.Count.ShouldBe(2);
    }

    // --- RemoveItem ---

    [Fact]
    public void RemoveItem_WhenItemExists_RemovesIt()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var cartItem = cart.AddItem(ActiveProduct(), 1m, 20m);

        cart.RemoveItem(cartItem.Id);

        cart.CartItems.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveItem_WhenItemNotFound_ThrowsDomainException()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());

        Should.Throw<DomainException>(() => cart.RemoveItem(Guid.NewGuid()))
            .Message.ShouldContain("bulunamadı");
    }

    // --- Calculate ---

    [Fact]
    public void Calculate_WhenEmptyCart_ReturnZero()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());

        var total = cart.Calculate();

        total.ShouldBe(0m);
        cart.TotalCost.ShouldBe(0m);
    }

    [Fact]
    public void Calculate_SumsAllCartItemTotalPrices()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        cart.AddItem(ActiveProduct("Süt"), 2m, 30m);   // 60
        cart.AddItem(ActiveProduct("Ekmek"), 3m, 10m); // 30

        var total = cart.Calculate();

        total.ShouldBe(90m);
        cart.TotalCost.ShouldBe(90m);
    }

    [Fact]
    public void Calculate_RaisesShoppingCartCalculatedDomainEvent()
    {
        var eventId = Guid.NewGuid();
        var cart = ShoppingCart.Create(eventId);
        cart.AddItem(ActiveProduct(), 1m, 50m);

        cart.Calculate();

        cart.DomainEvents.Count.ShouldBe(1);
        var domainEvent = cart.DomainEvents.First().ShouldBeOfType<ShoppingCartCalculatedDomainEvent>();
        domainEvent.ShoppingCartId.ShouldBe(cart.Id);
        domainEvent.RelatedEventId.ShouldBe(eventId);
        domainEvent.TotalCost.ShouldBe(50m);
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
        domainEvent.OccurredOn.ShouldNotBe(default);
    }

    [Fact]
    public void Calculate_UpdatesTotalCostOnCart()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        cart.AddItem(ActiveProduct(), 4m, 25m); // 100

        cart.Calculate();

        cart.TotalCost.ShouldBe(100m);
    }
}
