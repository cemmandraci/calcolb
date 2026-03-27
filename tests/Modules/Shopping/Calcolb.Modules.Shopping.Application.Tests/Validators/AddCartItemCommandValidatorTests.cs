using Calcolb.Modules.Shopping.Application.Commands.AddCartItem;
using FluentValidation.TestHelper;

namespace Calcolb.Modules.Shopping.Application.Tests.Validators;

public class AddCartItemCommandValidatorTests
{
    private readonly AddCartItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidCommand_HasNoErrors()
    {
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 2m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenShoppingCartIdIsEmpty_HasShoppingCartIdError()
    {
        var command = new AddCartItemCommand(Guid.Empty, Guid.NewGuid(), 2m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ShoppingCartId)
            .WithErrorMessage("Sepet kimliği zorunludur.");
    }

    [Fact]
    public void Validate_WhenProductIdIsEmpty_HasProductIdError()
    {
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.Empty, 2m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductId)
            .WithErrorMessage("Ürün kimliği zorunludur.");
    }

    [Fact]
    public void Validate_WhenQuantityIsZero_HasQuantityError()
    {
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Miktar sıfırdan büyük olmalıdır.");
    }

    [Fact]
    public void Validate_WhenQuantityIsNegative_HasQuantityError()
    {
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), -1m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WhenQuantityIsDecimal_HasNoErrors()
    {
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 0.5m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenAllFieldsInvalid_HasMultipleErrors()
    {
        var command = new AddCartItemCommand(Guid.Empty, Guid.Empty, 0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ShoppingCartId);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
