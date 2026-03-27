using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Shopping.Infrastructure.Persistence.Repositories;

public sealed class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly ShoppingDbContext _dbContext;

    public ShoppingCartRepository(ShoppingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShoppingCarts
            .Include(sc => sc.CartItems)
            .FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
    }

    public async Task AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        await _dbContext.ShoppingCarts.AddAsync(shoppingCart, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        _dbContext.ShoppingCarts.Update(shoppingCart);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
