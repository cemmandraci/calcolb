using FluentValidation;

namespace Calcolb.Modules.Expense.Application.Commands.AddExpenseItem;

public sealed class AddExpenseItemCommandValidator : AbstractValidator<AddExpenseItemCommand>
{
    public AddExpenseItemCommandValidator()
    {
        RuleFor(x => x.ExpensePlanId)
            .NotEmpty().WithMessage("Gider planı kimliği zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Gider kalemi adı zorunludur.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Gider tutarı sıfırdan büyük olmalıdır.");
    }
}
