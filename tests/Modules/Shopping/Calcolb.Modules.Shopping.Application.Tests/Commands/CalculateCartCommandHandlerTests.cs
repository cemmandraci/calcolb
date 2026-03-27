using Calcolb.Modules.Shopping.Application.Commands.CalculateCart;
using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Shopping.Application.Tests.Commands;

public class CalculateCartCommandHandlerTests
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly CalculateCartCommandHandler _handler;

    public CalculateCartCommandHandlerTests()
    {
        _cartRepository = Substitute.For<IShoppingCartRepository>();
        _handler = new CalculateCartCommandHandler(_cartRepository);
    }

    private static Product ActiveProduct(string name = "Ürün") =>
        Product.Create(name, Guid.NewGuid(), "111", "adet", "Migros", true);

    [Fact]
    public async Task Handle_WhenEmptyCart_ReturnsTotalCostZero()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);

        var result = await _handler.Handle(new CalculateCartCommand(cart.Id), CancellationToken.None);

        result.TotalCost.ShouldBe(0m);
    }

    [Fact]
    public async Task Handle_WhenCartHasItems_ReturnsTotalCost()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        cart.AddItem(ActiveProduct("Süt"), 2m, 30m);   // 60
        cart.AddItem(ActiveProduct("Ekmek"), 3m, 10m); // 30
        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);

        var result = await _handler.Handle(new CalculateCartCommand(cart.Id), CancellationToken.None);

        result.TotalCost.ShouldBe(90m);
    }

    [Fact]
    public async Task Handle_WhenCartFound_CallsRepositoryUpdateAsync()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid());
        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>()).Returns(cart);

        await _handler.Handle(new CalculateCartCommand(cart.Id), CancellationToken.None);

        await _cartRepository.Received(1).UpdateAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCartNotFound_ThrowsDomainException()
    {
        _cartRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ShoppingCart?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new CalculateCartCommand(Guid.NewGuid()),
                CancellationToken.None).AsTask());
    }
}
