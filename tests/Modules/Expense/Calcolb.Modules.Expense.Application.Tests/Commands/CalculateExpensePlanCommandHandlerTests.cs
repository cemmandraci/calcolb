using Calcolb.Modules.Expense.Application.Commands.CalculateExpensePlan;
using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Modules.Expense.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Expense.Application.Tests.Commands;

public class CalculateExpensePlanCommandHandlerTests
{
    private readonly IExpensePlanRepository _repository;
    private readonly CalculateExpensePlanCommandHandler _handler;

    public CalculateExpensePlanCommandHandlerTests()
    {
        _repository = Substitute.For<IExpensePlanRepository>();
        _handler = new CalculateExpensePlanCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenEmptyPlan_ReturnsTotalCostZero()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new CalculateExpensePlanCommand(plan.Id), CancellationToken.None);

        result.TotalCost.ShouldBe(0m);
        result.BufferApplied.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenBufferDisabled_ReturnsSumWithoutBuffer()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        plan.AddItem("Kömür", 500m);
        plan.AddItem("Çadır", 1000m);
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new CalculateExpensePlanCommand(plan.Id), CancellationToken.None);

        result.TotalCost.ShouldBe(1500m);
        result.BufferApplied.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenBufferEnabled_ReturnsTotalWithTenPercent()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        plan.AddItem("Erzak", 1000m);
        plan.EnableBuffer();
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new CalculateExpensePlanCommand(plan.Id), CancellationToken.None);

        // 1000 * 1.10 = 1100
        result.TotalCost.ShouldBe(1100m);
        result.BufferApplied.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenPlanFound_CallsRepositoryUpdateAsync()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        await _handler.Handle(new CalculateExpensePlanCommand(plan.Id), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(plan, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ThrowsDomainException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ExpensePlan?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new CalculateExpensePlanCommand(Guid.NewGuid()),
                CancellationToken.None).AsTask());
    }
}
