using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler : ICommandHandler<RemoveCartItemCommand, RemoveCartItemResult>
{
    private readonly IShoppingCartRepository _cartRepository;

    public RemoveCartItemCommandHandler(IShoppingCartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async ValueTask<RemoveCartItemResult> Handle(RemoveCartItemCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(command.ShoppingCartId, cancellationToken)
            ?? throw new DomainException("Sepet bulunamadı.");

        cart.RemoveItem(command.CartItemId);

        await _cartRepository.UpdateAsync(cart, cancellationToken);

        return new RemoveCartItemResult(true);
    }
}
