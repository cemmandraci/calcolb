using Calcolb.Modules.Event.Application.Commands.CreateEvent;
using FluentValidation.TestHelper;

namespace Calcolb.Modules.Event.Application.Tests.Validators;

public class CreateEventCommandValidatorTests
{
    private readonly CreateEventCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidCommand_HasNoErrors()
    {
        var command = new CreateEventCommand("Piknik", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Piknik")]
    [InlineData("Kamp")]
    [InlineData("DogumGünü")]
    [InlineData("EvPartisi")]
    [InlineData("SporOutdoor")]
    public void Validate_WhenValidEventType_HasNoEventTypeErrors(string eventType)
    {
        var command = new CreateEventCommand(eventType, 1, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.EventType);
    }

    [Fact]
    public void Validate_WhenEventTypeIsEmpty_HasEventTypeError()
    {
        var command = new CreateEventCommand("", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EventType)
            .WithErrorMessage("Etkinlik tipi zorunludur.");
    }

    [Fact]
    public void Validate_WhenEventTypeIsInvalid_HasEventTypeError()
    {
        var command = new CreateEventCommand("GeçersizTip", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EventType)
            .WithErrorMessage("Geçersiz etkinlik tipi.");
    }

    [Fact]
    public void Validate_WhenParticipantCountIsZero_HasParticipantCountError()
    {
        var command = new CreateEventCommand("Piknik", 0, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ParticipantCount)
            .WithErrorMessage("Katılımcı sayısı en az 1 olmalıdır.");
    }

    [Fact]
    public void Validate_WhenParticipantCountIsNegative_HasParticipantCountError()
    {
        var command = new CreateEventCommand("Piknik", -1, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ParticipantCount);
    }

    [Fact]
    public void Validate_WhenParticipantCountIsOne_HasNoParticipantCountError()
    {
        var command = new CreateEventCommand("Piknik", 1, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ParticipantCount);
    }

    [Fact]
    public void Validate_WhenEventDateIsNull_HasNoErrors()
    {
        var command = new CreateEventCommand("Kamp", 10, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenAllFieldsInvalid_HasMultipleErrors()
    {
        var command = new CreateEventCommand("", 0, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EventType);
        result.ShouldHaveValidationErrorFor(x => x.ParticipantCount);
    }
}
