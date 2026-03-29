using Calcolb.Modules.Expense.Domain.Entities;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Expense.Domain.Tests.Entities;

public class ExpenseItemTests
{
    [Fact]
    public void Create_WhenValidParameters_ReturnsExpenseItem()
    {
        var planId = Guid.NewGuid();

        var item = ExpenseItem.Create(planId, "Kömür", 500m);

        item.ShouldNotBeNull();
        item.Id.ShouldNotBe(Guid.Empty);
        item.ExpensePlanId.ShouldBe(planId);
        item.Name.ShouldBe("Kömür");
        item.Amount.ShouldBe(500m);
    }

    [Fact]
    public void Create_WhenAmountIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            ExpenseItem.Create(Guid.NewGuid(), "Test", 0m))
            .Message.ShouldContain("sıfırdan büyük");
    }

    [Fact]
    public void Create_WhenAmountIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            ExpenseItem.Create(Guid.NewGuid(), "Test", -100m))
            .Message.ShouldContain("sıfırdan büyük");
    }

    [Fact]
    public void Create_WhenNameIsEmpty_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            ExpenseItem.Create(Guid.NewGuid(), "", 100m));
    }

    [Fact]
    public void Create_WhenNameIsWhitespace_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            ExpenseItem.Create(Guid.NewGuid(), "   ", 100m));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var i1 = ExpenseItem.Create(Guid.NewGuid(), "A", 100m);
        var i2 = ExpenseItem.Create(Guid.NewGuid(), "B", 200m);

        i1.Id.ShouldNotBe(i2.Id);
    }
}
