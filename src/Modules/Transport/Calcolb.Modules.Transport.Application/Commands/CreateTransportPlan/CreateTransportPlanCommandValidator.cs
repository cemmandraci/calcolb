using Calcolb.Modules.Transport.Domain.ValueObjects;
using FluentValidation;

namespace Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan;

public sealed class CreateTransportPlanCommandValidator : AbstractValidator<CreateTransportPlanCommand>
{
    public CreateTransportPlanCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Etkinlik kimliği zorunludur.");

        RuleFor(x => x.TransportType)
            .NotEmpty().WithMessage("Ulaşım tipi zorunludur.")
            .Must(BeAValidTransportType).WithMessage("Geçersiz ulaşım tipi.");

        RuleFor(x => x.ParticipantCount)
            .GreaterThan(0).WithMessage("Katılımcı sayısı en az 1 olmalıdır.");

        RuleFor(x => x.CostPerPerson)
            .GreaterThan(0).WithMessage("Kişi başı bilet ücreti sıfırdan büyük olmalıdır.")
            .When(x => x.CostPerPerson.HasValue);
    }

    private static bool BeAValidTransportType(string transportType)
    {
        return TransportType.GetAll().Any(t => t.Value.Equals(transportType, StringComparison.OrdinalIgnoreCase));
    }
}
