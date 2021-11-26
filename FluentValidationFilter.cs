namespace GreenPipes.FluentValidation;

public class FluentValidationFilter<TContext> : IFilter<TContext>
where TContext : class, PipeContext
{
    readonly IPipe<ValidationFailureContext<TContext>> _validationFailurePipe;

    public FluentValidationFilter(IPipe<ValidationFailureContext<TContext>> validationFailurePipe)
    {
        _validationFailurePipe = validationFailurePipe;
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateScope("FluentValidation");
        _validationFailurePipe.Probe(scope);
    }

    public async Task Send(TContext context, IPipe<TContext> next)
    {

        if (context.TryGetPayload(out IEnumerable<IValidator<TContext>> validators))
        {
            var failures = validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                await _validationFailurePipe.Send(new ValidationFailureContext<TContext>(context, failures));
            }

            await next.Send(context);
        }
    }
}