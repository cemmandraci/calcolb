using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.CalculateCart;

public sealed class CalculateCartCommandHandler : ICommandHandler<CalculateCartCommand, CalculateCartResult>
{
    private readonly IShoppingCartRepository _cartRepository;

    public CalculateCartCommandHandler(IShoppingCartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async ValueTask<CalculateCartResult> Handle(CalculateCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(command.ShoppingCartId, cancellationToken)
            ?? throw new DomainException("Sepet bulunamadı.");

        var totalCost = cart.Calculate();

        await _cartRepository.UpdateAsync(cart, cancellationToken);

        return new CalculateCartResult(totalCost);
    }
}
