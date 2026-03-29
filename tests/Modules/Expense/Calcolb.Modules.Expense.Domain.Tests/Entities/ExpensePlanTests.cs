using Calcolb.Modules.Expense.Domain.Entities;
using Calcolb.Modules.Expense.Domain.Events;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Expense.Domain.Tests.Entities;

public class ExpensePlanTests
{
    private readonly Guid _eventId = Guid.NewGuid();

    // --- Create ---

    [Fact]
    public void Create_WhenValidEventId_ReturnsExpensePlan()
    {
        var plan = ExpensePlan.Create(_eventId);

        plan.ShouldNotBeNull();
        plan.Id.ShouldNotBe(Guid.Empty);
        plan.EventId.ShouldBe(_eventId);
        plan.BufferEnabled.ShouldBeFalse();
        plan.BufferPercentage.ShouldBe(10m);
        plan.TotalCost.ShouldBe(0m);
        plan.ExpenseItems.ShouldBeEmpty();
    }

    [Fact]
    public void Create_WhenEventIdIsEmpty_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => ExpensePlan.Create(Guid.Empty));
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var p1 = ExpensePlan.Create(_eventId);
        var p2 = ExpensePlan.Create(_eventId);

        p1.Id.ShouldNotBe(p2.Id);
    }

    // --- AddItem ---

    [Fact]
    public void AddItem_WhenValidParameters_AddsExpenseItem()
    {
        var plan = ExpensePlan.Create(_eventId);

        var item = plan.AddItem("Kömür", 500m);

        plan.ExpenseItems.Count.ShouldBe(1);
        item.Name.ShouldBe("Kömür");
        item.Amount.ShouldBe(500m);
        item.ExpensePlanId.ShouldBe(plan.Id);
    }

    [Fact]
    public void AddItem_MultipleItems_AddsAll()
    {
        var plan = ExpensePlan.Create(_eventId);

        plan.AddItem("Kömür", 500m);
        plan.AddItem("Çadır", 1000m);
        plan.AddItem("Erzak", 750m);

        plan.ExpenseItems.Count.ShouldBe(3);
    }

    [Fact]
    public void AddItem_WhenAmountIsZero_ThrowsDomainException()
    {
        var plan = ExpensePlan.Create(_eventId);

        Should.Throw<DomainException>(() => plan.AddItem("Test", 0m));
    }

    // --- RemoveItem ---

    [Fact]
    public void RemoveItem_WhenItemExists_RemovesIt()
    {
        var plan = ExpensePlan.Create(_eventId);
        var item = plan.AddItem("Kömür", 500m);

        plan.RemoveItem(item.Id);

        plan.ExpenseItems.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveItem_WhenItemNotFound_ThrowsDomainException()
    {
        var plan = ExpensePlan.Create(_eventId);

        Should.Throw<DomainException>(() => plan.RemoveItem(Guid.NewGuid()))
            .Message.ShouldContain("bulunamadı");
    }

    // --- ToggleBuffer ---

    [Fact]
    public void ToggleBuffer_WhenDisabled_EnablesBuffer()
    {
        var plan = ExpensePlan.Create(_eventId);

        plan.ToggleBuffer();

        plan.BufferEnabled.ShouldBeTrue();
    }

    [Fact]
    public void ToggleBuffer_WhenEnabled_DisablesBuffer()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.EnableBuffer();

        plan.ToggleBuffer();

        plan.BufferEnabled.ShouldBeFalse();
    }

    // --- Calculate (no buffer) ---

    [Fact]
    public void Calculate_WhenBufferDisabled_ReturnsSumOfItems()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.AddItem("Kömür", 500m);
        plan.AddItem("Çadır", 1000m);

        var total = plan.Calculate();

        total.ShouldBe(1500m);
        plan.TotalCost.ShouldBe(1500m);
    }

    [Fact]
    public void Calculate_WhenEmptyAndBufferDisabled_ReturnsZero()
    {
        var plan = ExpensePlan.Create(_eventId);

        var total = plan.Calculate();

        total.ShouldBe(0m);
    }

    // --- Calculate (with buffer) ---

    [Fact]
    public void Calculate_WhenBufferEnabled_AddsBufferToTotal()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.AddItem("Erzak", 1000m);
        plan.EnableBuffer();

        var total = plan.Calculate();

        // 1000 * 1.10 = 1100
        total.ShouldBe(1100m);
        plan.TotalCost.ShouldBe(1100m);
    }

    [Fact]
    public void Calculate_WhenBufferEnabledWithMultipleItems_CalculatesCorrectly()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.AddItem("Kömür", 500m);
        plan.AddItem("Çadır", 1000m);
        plan.AddItem("Erzak", 750m);
        plan.EnableBuffer();

        var total = plan.Calculate();

        // (500 + 1000 + 750) * 1.10 = 2250 * 1.10 = 2475
        total.ShouldBe(2475m);
    }

    [Fact]
    public void Calculate_WhenBufferDisabledAfterEnabled_DoesNotApplyBuffer()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.AddItem("Erzak", 1000m);
        plan.EnableBuffer();
        plan.DisableBuffer();

        var total = plan.Calculate();

        total.ShouldBe(1000m);
    }

    // --- Domain Event ---

    [Fact]
    public void Calculate_RaisesExpensePlanCalculatedDomainEvent()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.AddItem("Kömür", 500m);

        plan.Calculate();

        plan.DomainEvents.Count.ShouldBe(1);
        var domainEvent = plan.DomainEvents.First().ShouldBeOfType<ExpensePlanCalculatedDomainEvent>();
        domainEvent.ExpensePlanId.ShouldBe(plan.Id);
        domainEvent.RelatedEventId.ShouldBe(_eventId);
        domainEvent.TotalCost.ShouldBe(500m);
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
        domainEvent.OccurredOn.ShouldNotBe(default);
    }

    [Fact]
    public void Calculate_WhenBufferEnabled_DomainEventContainsBufferedTotal()
    {
        var plan = ExpensePlan.Create(_eventId);
        plan.AddItem("Erzak", 1000m);
        plan.EnableBuffer();

        plan.Calculate();

        var domainEvent = plan.DomainEvents.First().ShouldBeOfType<ExpensePlanCalculatedDomainEvent>();
        domainEvent.TotalCost.ShouldBe(1100m);
    }
}
