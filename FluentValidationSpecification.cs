namespace GreenPipes.FluentValidation;

public class FluentValidationSpecification<TContext> : IPipeSpecification<TContext>
 where TContext : class, PipeContext
{
    readonly IPipe<ValidationFailureContext<TContext>> _validationFailurePipe;

    public FluentValidationSpecification(IPipe<ValidationFailureContext<TContext>> validationValidationFailurePipe)
    {
        _validationFailurePipe = validationValidationFailurePipe;
    }

    public IEnumerable<ValidationResult> Validate()
    {
        if (_validationFailurePipe == null)
            yield return this.Failure("validationValidationFailurePipe", "You must provide a failure route");
    }

    public void Apply(IPipeBuilder<TContext> builder)
    {
        builder.AddFilter(new FluentValidationFilter<TContext>(_validationFailurePipe));
    }
}
