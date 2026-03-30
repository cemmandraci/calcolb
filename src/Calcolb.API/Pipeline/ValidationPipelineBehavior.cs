using FluentValidation;
using Mediator;

namespace Calcolb.API.Pipeline;

public sealed class ValidationPipelineBehavior<TMessage, TResponse>
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly IEnumerable<IValidator<TMessage>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(message, cancellationToken);

        var context = new ValidationContext<TMessage>(message);

        var failures = _validators
            .Select(v => v.Validate(context))
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next(message, cancellationToken);
    }
}
