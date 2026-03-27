using Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan;
using FluentValidation.TestHelper;

namespace Calcolb.Modules.Transport.Application.Tests.Validators;

public class CreateTransportPlanCommandValidatorTests
{
    private readonly CreateTransportPlanCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidCommand_HasNoErrors()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Arabayla", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Arabayla")]
    [InlineData("TopluTasima")]
    [InlineData("Yuruyerek")]
    public void Validate_WhenValidTransportType_HasNoTransportTypeErrors(string transportType)
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), transportType, 1, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TransportType);
    }

    [Fact]
    public void Validate_WhenTransportTypeIsEmpty_HasTransportTypeError()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TransportType)
            .WithErrorMessage("Ulaşım tipi zorunludur.");
    }

    [Fact]
    public void Validate_WhenTransportTypeIsInvalid_HasTransportTypeError()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Bisiklet", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TransportType)
            .WithErrorMessage("Geçersiz ulaşım tipi.");
    }

    [Fact]
    public void Validate_WhenParticipantCountIsZero_HasParticipantCountError()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Arabayla", 0, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ParticipantCount)
            .WithErrorMessage("Katılımcı sayısı en az 1 olmalıdır.");
    }

    [Fact]
    public void Validate_WhenParticipantCountIsNegative_HasParticipantCountError()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Arabayla", -1, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ParticipantCount);
    }

    [Fact]
    public void Validate_WhenEventIdIsEmpty_HasEventIdError()
    {
        var command = new CreateTransportPlanCommand(Guid.Empty, "Arabayla", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EventId)
            .WithErrorMessage("Etkinlik kimliği zorunludur.");
    }

    [Fact]
    public void Validate_WhenCostPerPersonIsZero_HasCostPerPersonError()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "TopluTasima", 5, 0m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CostPerPerson)
            .WithErrorMessage("Kişi başı bilet ücreti sıfırdan büyük olmalıdır.");
    }

    [Fact]
    public void Validate_WhenCostPerPersonIsNull_HasNoCostPerPersonError()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Arabayla", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CostPerPerson);
    }

    [Fact]
    public void Validate_WhenAllFieldsInvalid_HasMultipleErrors()
    {
        var command = new CreateTransportPlanCommand(Guid.Empty, "", 0, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EventId);
        result.ShouldHaveValidationErrorFor(x => x.TransportType);
        result.ShouldHaveValidationErrorFor(x => x.ParticipantCount);
    }
}
