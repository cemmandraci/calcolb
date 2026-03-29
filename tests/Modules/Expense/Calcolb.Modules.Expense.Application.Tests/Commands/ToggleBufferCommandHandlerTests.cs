using Calcolb.Modules.Expense.Application.Commands.ToggleBuffer;
using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Modules.Expense.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Expense.Application.Tests.Commands;

public class ToggleBufferCommandHandlerTests
{
    private readonly IExpensePlanRepository _repository;
    private readonly ToggleBufferCommandHandler _handler;

    public ToggleBufferCommandHandlerTests()
    {
        _repository = Substitute.For<IExpensePlanRepository>();
        _handler = new ToggleBufferCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenBufferDisabled_EnablesBufferAndReturnsTrue()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new ToggleBufferCommand(plan.Id), CancellationToken.None);

        result.BufferEnabled.ShouldBeTrue();
        plan.BufferEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenBufferEnabled_DisablesBufferAndReturnsFalse()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        plan.EnableBuffer();
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new ToggleBufferCommand(plan.Id), CancellationToken.None);

        result.BufferEnabled.ShouldBeFalse();
        plan.BufferEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_CallsRepositoryUpdateAsync()
    {
        var plan = ExpensePlan.Create(Guid.NewGuid());
        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        await _handler.Handle(new ToggleBufferCommand(plan.Id), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(plan, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ThrowsDomainException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ExpensePlan?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new ToggleBufferCommand(Guid.NewGuid()),
                CancellationToken.None).AsTask());
    }
}
