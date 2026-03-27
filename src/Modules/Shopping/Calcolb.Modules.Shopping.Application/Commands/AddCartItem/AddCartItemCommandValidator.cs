using FluentValidation;

namespace Calcolb.Modules.Shopping.Application.Commands.AddCartItem;

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.ShoppingCartId)
            .NotEmpty().WithMessage("Sepet kimliği zorunludur.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Ürün kimliği zorunludur.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar sıfırdan büyük olmalıdır.");
    }
}
