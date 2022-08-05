# FluentValidationForMassTransit
 Allows functionality from the FluentValidation libraries to used in a GreenPipes (MassTransit) pipeline. This means that any messages (e.g. commands and queries) that
 pass through your pipeline will be validated if a validator exists for that message type, otherwise they won't.
 # Get Started
 1. Install the Nuget package `FluentValidationForMassTransit`
 2. In your Startup.cs file, in your `ConfigureServices` method, add FluentValidation and register your validators as per the FluentValidation documentation:
 ```
        services.AddControllers()
            .AddFluentValidation(configuration => configuration
            .RegisterValidatorsFromAssemblyContaining<SomeValidator>());
 ```
 3. Decide what you would like to happen when a message fails validation. Make a `ValidationFailurePipe` to handle those messages. Your `ValidationFailurePipe`
 *must* implement `FluentValidationForMassTransit.IValidationFailurePipe` (an interface included in this package). It *can optionally* inherit from
`FluentValidationForMassTransit.ValidationFailurePipeBase` (a base class included in this package). Here is an example of a `ValidationFailurePipe`
that passes the dictionary of validation errors back to the caller, but you can code whatever functionality you like. In most cases you'll want to be calling
`context.InnerContext.RespondAsync`. The context's `InnerContext` is the `ConsumeContext` of the message that was validated.
```
public class ValidationFailurePipe<TMessage> : ValidationFailurePipeBase<TMessage>
    where TMessage : class
{

    public async override Task Send(ValidationFailureContext<TMessage> context)
    {
        var validationProblems = context.ValidationProblems;
        await context.InnerContext.RespondAsync(validationProblems);
    }
}
```
4. Register your `ValidationFailurePipe` in `Startup.ConfigureServices` with a transient lifetime.
```
        services.AddTransient(
            typeof(IValidationFailurePipe<>),
            typeof(ValidationFailurePipe<>));
```
5. In `Startup.ConfigureServices`, when you call `AddMassTransit`, you will then specify your transport mode on the `IServiceCollectionBusConfigurator`
such as `UsingInMemory` or `UsingRabbitMQ` etc. Through that, you can use the fluent API to get an instance of `IReceiveEndpointConfigurator`.
That is where you can specify (using the extension method in this package) `UseFluentValidationForMassTransit`. Pass as an arguemnt your instance of 
`IBusRegistrationContext`. An example is below:
```
        services.AddMassTransit(services =>
        {
            // Add consumers

            // Add request clients

            services.UsingRabbitMq((registrationContext, factoryConfigurator) =>
            {
                factoryConfigurator.ReceiveEndpoint(AssemblyName, endpointConfigurator =>
                {
                    endpointConfigurator.UseFluentValidationForMassTransit(registrationContext);
                    // Configure consumers
                });
            });
        });
```
6. From now on, if the FluentValidation library finds a validator that matches the message type, it will use it to validate the message. If a message
passes validation, it will be sent to the next handler in the pipeline. If it fails, it will be passed to your ValidationFailurePipe to be handled.
Below is an example of a validator:
```
public class SomeValidator : AbstractValidator<ISomeCommand>
{
    public SomeValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage(command =>
        $"'{command.OrderId}' is not a valid identifier.");

        RuleFor(x => x.Items).NotEmpty().WithMessage(command =>
        $"Order { command.OrderId} has no items.");
    }
}
```
