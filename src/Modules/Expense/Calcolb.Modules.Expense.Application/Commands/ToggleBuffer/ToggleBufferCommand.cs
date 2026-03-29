using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.ToggleBuffer;

public sealed record ToggleBufferCommand(Guid ExpensePlanId) : ICommand<ToggleBufferResult>;

public sealed record ToggleBufferResult(bool BufferEnabled);
