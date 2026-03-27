using Calcolb.Modules.Shopping.Application.Commands.AddCartItem;
using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Application.Services;
using Calcolb.Modules.Shopping.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Shopping.Application.Tests.Commands;

public class AddCartItemCommandHandlerTests
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPriceProvider _priceProvider;
    private readonly AddCartItemCommandHandler _handler;

    public AddCartItemCommandHandlerTests()
    {
        _cartRepository = Substitute.For<IShoppingCartRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _priceProvider = Substitute.For<IPriceProvider>();
        _handler = new AddCartItemCommandHandler(_cartRepository, _productRepository, _priceProvider);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ReturnsCartItemId()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = Product.Create("Süt", Guid.NewGuid(), "111", "litre", "Migros", true);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _priceProvider.GetPriceAsync(product.Barcode, product.MarketType, Arg.Any<CancellationToken>()).Returns(30m);

        var result = await _handler.Handle(
            new AddCartItemCommand(cart.Id, product.Id, 2m), CancellationToken.None);

        result.ShouldNotBeNull();
        result.CartItemId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_AddsItemToCart()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = Product.Create("Ekmek", Guid.NewGuid(), "222", "adet", "Bim", true);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _priceProvider.GetPriceAsync(product.Barcode, product.MarketType, Arg.Any<CancellationToken>()).Returns(10m);

        await _handler.Handle(new AddCartItemCommand(cart.Id, product.Id, 3m), CancellationToken.None);

        cart.CartItems.Count.ShouldBe(1);
        cart.CartItems[0].Quantity.ShouldBe(3m);
        cart.CartItems[0].UnitPrice.ShouldBe(10m);
        cart.CartItems[0].TotalPrice.ShouldBe(30m);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CallsRepositoryUpdateAsync()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = Product.Create("Yoğurt", Guid.NewGuid(), "333", "kg", "Migros", true);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _priceProvider.GetPriceAsync(product.Barcode, product.MarketType, Arg.Any<CancellationToken>()).Returns(45m);

        await _handler.Handle(new AddCartItemCommand(cart.Id, product.Id, 1m), CancellationToken.None);

        await _cartRepository.Received(1).UpdateAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCartNotFound_ThrowsDomainException()
    {
        _cartRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ShoppingCart?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 1m),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ThrowsDomainException()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new AddCartItemCommand(cart.Id, Guid.NewGuid(), 1m),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WhenInactiveProduct_ThrowsDomainException()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var inactiveProduct = Product.Create("Bayat Ekmek", Guid.NewGuid(), "444", "adet", "Bim", false);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _productRepository.GetByIdAsync(inactiveProduct.Id, Arg.Any<CancellationToken>())
            .Returns(inactiveProduct);
        _priceProvider.GetPriceAsync(inactiveProduct.Barcode, inactiveProduct.MarketType, Arg.Any<CancellationToken>())
            .Returns(5m);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new AddCartItemCommand(cart.Id, inactiveProduct.Id, 1m),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_FetchesPriceFromProvider()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        var product = Product.Create("Peynir", Guid.NewGuid(), "555", "kg", "Carrefour", true);

        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _priceProvider.GetPriceAsync(product.Barcode, product.MarketType, Arg.Any<CancellationToken>()).Returns(120m);

        await _handler.Handle(new AddCartItemCommand(cart.Id, product.Id, 1m), CancellationToken.None);

        await _priceProvider.Received(1).GetPriceAsync("555", "Carrefour", Arg.Any<CancellationToken>());
    }
}
