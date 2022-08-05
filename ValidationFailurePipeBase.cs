namespace MassTransit;

public abstract class ValidationFailurePipeBase<TMessage> : IValidationFailurePipe<TMessage>
    where TMessage : class
{
    public virtual void Probe(ProbeContext context)
    {
        context.CreateScope($"ValidationFailurePipe<{typeof(TMessage)}>");
    }

    public abstract Task Send(ValidationFailureContext<TMessage> context);
}