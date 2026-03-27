using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Application.Services;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.AddCartItem;

public sealed class AddCartItemCommandHandler : ICommandHandler<AddCartItemCommand, AddCartItemResult>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPriceProvider _priceProvider;

    public AddCartItemCommandHandler(
        IShoppingCartRepository cartRepository,
        IProductRepository productRepository,
        IPriceProvider priceProvider)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _priceProvider = priceProvider;
    }

    public async ValueTask<AddCartItemResult> Handle(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(command.ShoppingCartId, cancellationToken)
            ?? throw new DomainException("Sepet bulunamadı.");

        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new DomainException("Ürün bulunamadı.");

        var unitPrice = await _priceProvider.GetPriceAsync(product.Barcode, product.MarketType, cancellationToken);

        var cartItem = cart.AddItem(product, command.Quantity, unitPrice);

        await _cartRepository.UpdateAsync(cart, cancellationToken);

        return new AddCartItemResult(cartItem.Id);
    }
}
