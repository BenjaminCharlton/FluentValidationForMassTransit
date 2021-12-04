namespace FluentValidationForMassTransit;

public interface IValidationFailurePipe<TMessage> :
    IPipe<ValidationFailureContext<TMessage>>
    where TMessage : class
{ }