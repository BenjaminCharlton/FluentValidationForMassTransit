namespace MassTransit;

public class FluentValidationFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly IValidationFailurePipe<TMessage> _failurePipe;
    private readonly IValidator<TMessage>? _validator;

    public FluentValidationFilter(IValidator<TMessage>? validator, IValidationFailurePipe<TMessage> failurePipe)
    {
        _validator = validator;
        _failurePipe = failurePipe ?? throw new ArgumentNullException(nameof(failurePipe));
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("FluentValidationFilter");
    }


    public async Task Send(
        ConsumeContext<TMessage> context,
        IPipe<ConsumeContext<TMessage>> next)
    {
        if (_validator is null)
        {
            await next.Send(context);
            return;
        }

        var message = context.Message;
        var validationResult = await _validator.ValidateAsync(message, context.CancellationToken);

        if (validationResult.IsValid)
        {
            await next.Send(context);
            return;
        }

        var validationProblems = validationResult.Errors.ToErrorDictionary();

        var failureContext = new ValidationFailureContext<TMessage>(context, validationProblems);
        await _failurePipe.Send(failureContext);
    }
}