using Calcolb.Modules.Estimation.Application.Commands.CreateEstimationSession;
using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Estimation.Application.Tests.Commands;

public class CreateEstimationSessionCommandHandlerTests
{
    private readonly IEstimationSessionRepository _repository;
    private readonly CreateEstimationSessionCommandHandler _handler;

    public CreateEstimationSessionCommandHandlerTests()
    {
        _repository = Substitute.For<IEstimationSessionRepository>();
        _handler = new CreateEstimationSessionCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSessionId()
    {
        var command = new CreateEstimationSessionCommand(Guid.NewGuid(), 5);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.SessionId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CallsRepositoryAddAsync()
    {
        var command = new CreateEstimationSessionCommand(Guid.NewGuid(), 3);

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(1).AddAsync(Arg.Any<EstimationSession>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithZeroParticipants_ThrowsDomainException()
    {
        var command = new CreateEstimationSessionCommand(Guid.NewGuid(), 0);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask());
    }
}
