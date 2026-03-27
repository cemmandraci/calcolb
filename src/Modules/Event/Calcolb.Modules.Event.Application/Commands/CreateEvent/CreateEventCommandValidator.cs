using Calcolb.Modules.Event.Domain.ValueObjects;
using FluentValidation;

namespace Calcolb.Modules.Event.Application.Commands.CreateEvent;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.EventType)
            .NotEmpty().WithMessage("Etkinlik tipi zorunludur.")
            .Must(BeAValidEventType).WithMessage("Geçersiz etkinlik tipi.");

        RuleFor(x => x.ParticipantCount)
            .GreaterThan(0).WithMessage("Katılımcı sayısı en az 1 olmalıdır.");
    }

    private static bool BeAValidEventType(string eventType)
    {
        return EventType.GetAll().Any(t => t.Value.Equals(eventType, StringComparison.OrdinalIgnoreCase));
    }
}
