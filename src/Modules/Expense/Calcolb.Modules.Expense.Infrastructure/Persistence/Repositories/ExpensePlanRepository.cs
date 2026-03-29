using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Modules.Expense.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Expense.Infrastructure.Persistence.Repositories;

public sealed class ExpensePlanRepository : IExpensePlanRepository
{
    private readonly ExpenseDbContext _dbContext;

    public ExpensePlanRepository(ExpenseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpensePlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ExpensePlans
            .Include(ep => ep.ExpenseItems)
            .FirstOrDefaultAsync(ep => ep.Id == id, cancellationToken);
    }

    public async Task AddAsync(ExpensePlan expensePlan, CancellationToken cancellationToken = default)
    {
        await _dbContext.ExpensePlans.AddAsync(expensePlan, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ExpensePlan expensePlan, CancellationToken cancellationToken = default)
    {
        _dbContext.ExpensePlans.Update(expensePlan);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
