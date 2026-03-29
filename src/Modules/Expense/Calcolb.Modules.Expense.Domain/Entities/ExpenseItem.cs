using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Expense.Domain.Entities;

public sealed class ExpenseItem : Entity<Guid>
{
    public Guid ExpensePlanId { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Amount { get; private set; }

    private ExpenseItem() { }

    public static ExpenseItem Create(Guid expensePlanId, string name, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Gider kalemi adı zorunludur.");

        if (amount <= 0)
            throw new DomainException("Gider tutarı sıfırdan büyük olmalıdır.");

        return new ExpenseItem
        {
            Id = Guid.NewGuid(),
            ExpensePlanId = expensePlanId,
            Name = name,
            Amount = amount
        };
    }
}
