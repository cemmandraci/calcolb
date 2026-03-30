using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Domain.Entities;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.CreateShoppingCart;

public sealed class CreateShoppingCartCommandHandler
    : ICommandHandler<CreateShoppingCartCommand, CreateShoppingCartResult>
{
    private readonly IShoppingCartRepository _repository;

    public CreateShoppingCartCommandHandler(IShoppingCartRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<CreateShoppingCartResult> Handle(
        CreateShoppingCartCommand command,
        CancellationToken cancellationToken)
    {
        var cart = ShoppingCart.Create(command.EventId);

        await _repository.AddAsync(cart, cancellationToken);

        return new CreateShoppingCartResult(cart.Id);
    }
}
