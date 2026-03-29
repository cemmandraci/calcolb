using Calcolb.Modules.Expense.Application.Commands.AddExpenseItem;
using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Modules.Expense.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Expense.Application.Tests.Commands;

public class AddExpenseItemCommandHandlerTests
{
    private readonly IExpensePlanRepository _repository;
    private readonly AddExpenseItemCommandHandler _handler;

    public AddExpenseItemCommandHandlerTests()
    {
        _repository = Substitute.For<IExpensePlanRepository>();
        _handler = new AddExpenseItemCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ReturnsExpenseItemId()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new AddExpenseItemCommand(plan.Id, "Kömür", 500m), CancellationToken.None);

        result.ShouldNotBeNull();
        result.ExpenseItemId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_AddsItemToPlan()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        await _handler.Handle(
            new AddExpenseItemCommand(plan.Id, "Çadır", 1000m), CancellationToken.None);

        plan.ExpenseItems.Count.ShouldBe(1);
        plan.ExpenseItems[0].Name.ShouldBe("Çadır");
        plan.ExpenseItems[0].Amount.ShouldBe(1000m);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CallsRepositoryUpdateAsync()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        await _handler.Handle(
            new AddExpenseItemCommand(plan.Id, "Erzak", 750m), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(plan, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ThrowsDomainException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ExpensePlan?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new AddExpenseItemCommand(Guid.NewGuid(), "Test", 100m),
                CancellationToken.None).AsTask());
    }
}
