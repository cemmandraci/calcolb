using Calcolb.Modules.Expense.Application.Commands.AddExpenseItem;
using FluentValidation.TestHelper;

namespace Calcolb.Modules.Expense.Application.Tests.Validators;

public class AddExpenseItemCommandValidatorTests
{
    private readonly AddExpenseItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidCommand_HasNoErrors()
    {
        var command = new AddExpenseItemCommand(Guid.NewGuid(), "Kömür", 500m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenExpensePlanIdIsEmpty_HasExpensePlanIdError()
    {
        var command = new AddExpenseItemCommand(Guid.Empty, "Kömür", 500m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpensePlanId)
            .WithErrorMessage("Gider planı kimliği zorunludur.");
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_HasNameError()
    {
        var command = new AddExpenseItemCommand(Guid.NewGuid(), "", 500m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Gider kalemi adı zorunludur.");
    }

    [Fact]
    public void Validate_WhenAmountIsZero_HasAmountError()
    {
        var command = new AddExpenseItemCommand(Guid.NewGuid(), "Kömür", 0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Gider tutarı sıfırdan büyük olmalıdır.");
    }

    [Fact]
    public void Validate_WhenAmountIsNegative_HasAmountError()
    {
        var command = new AddExpenseItemCommand(Guid.NewGuid(), "Kömür", -100m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_WhenAllFieldsInvalid_HasMultipleErrors()
    {
        var command = new AddExpenseItemCommand(Guid.Empty, "", 0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ExpensePlanId);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_WhenAmountIsDecimal_HasNoErrors()
    {
        var command = new AddExpenseItemCommand(Guid.NewGuid(), "Erzak", 99.99m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
