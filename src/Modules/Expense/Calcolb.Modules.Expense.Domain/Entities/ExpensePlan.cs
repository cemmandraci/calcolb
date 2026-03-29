using Calcolb.Modules.Expense.Domain.Events;
using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Expense.Domain.Entities;

public sealed class ExpensePlan : AggregateRoot<Guid>
{
    public const decimal BufferRate = 0.10m;

    public Guid EventId { get; private set; }
    public bool BufferEnabled { get; private set; }
    public decimal BufferPercentage { get; private set; }
    public decimal TotalCost { get; private set; }

    private readonly List<ExpenseItem> _expenseItems = [];
    public IReadOnlyList<ExpenseItem> ExpenseItems => _expenseItems.AsReadOnly();

    private ExpensePlan() { }

    public static ExpensePlan Create(Guid eventId)
    {
        if (eventId == Guid.Empty)
            throw new DomainException("Etkinlik kimliği zorunludur.");

        return new ExpensePlan
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            BufferEnabled = false,
            BufferPercentage = BufferRate * 100
        };
    }

    public ExpenseItem AddItem(string name, decimal amount)
    {
        var item = ExpenseItem.Create(Id, name, amount);
        _expenseItems.Add(item);
        return item;
    }

    public void RemoveItem(Guid expenseItemId)
    {
        var item = _expenseItems.FirstOrDefault(i => i.Id == expenseItemId)
            ?? throw new DomainException("Gider kalemi bulunamadı.");

        _expenseItems.Remove(item);
    }

    public void EnableBuffer() => BufferEnabled = true;

    public void DisableBuffer() => BufferEnabled = false;

    public void ToggleBuffer() => BufferEnabled = !BufferEnabled;

    public decimal Calculate()
    {
        var subtotal = _expenseItems.Sum(i => i.Amount);
        TotalCost = BufferEnabled ? subtotal * (1 + BufferRate) : subtotal;

        RaiseDomainEvent(new ExpensePlanCalculatedDomainEvent(Id, EventId, TotalCost));

        return TotalCost;
    }
}
