using Calcolb.Modules.Expense.Domain.Entities;

namespace Calcolb.Modules.Expense.Application.Repositories;

public interface IExpensePlanRepository
{
    Task<ExpensePlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ExpensePlan expensePlan, CancellationToken cancellationToken = default);
    Task UpdateAsync(ExpensePlan expensePlan, CancellationToken cancellationToken = default);
}
