using Calcolb.Modules.Shopping.Domain.Entities;

namespace Calcolb.Modules.Shopping.Application.Repositories;

public interface IShoppingCartRepository
{
    Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
}
