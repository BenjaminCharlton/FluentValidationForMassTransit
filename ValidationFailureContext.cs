namespace GreenPipes.FluentValidation;

public class ValidationFailureContext<TWrappedContext> : BasePipeContext, PipeContext
where TWrappedContext : PipeContext
{
    public ValidationFailureContext(TWrappedContext wrappedContext, List<ValidationFailure> failures) : base(new ListPayloadCache())
    {
        WrappedContext = wrappedContext;
        Failures = failures;
    }

    public TWrappedContext WrappedContext { get; }
    public List<ValidationFailure> Failures { get; }
}
