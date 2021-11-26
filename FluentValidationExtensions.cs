namespace Microsoft.Extensions.DependencyInjection;

public static class FluentValidationExtensions
{
    public static void UseFluentValidation<TContext>(this IPipeConfigurator<TContext> configurator, IPipe<ValidationFailureContext<TContext>> validationFailurePipe)
        where TContext : class, PipeContext
    {
        configurator.AddPipeSpecification(new FluentValidationSpecification<TContext>(validationFailurePipe));
    }
}