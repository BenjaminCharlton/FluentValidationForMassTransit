namespace MassTransit;

/// <summary>
/// Extension methods for injecting functionality from the <see cref="FluentValidation"/> library into a <see cref="MassTransit"/> pipeline.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Registers the necessary filters to perform validation on message sent through the pipeline.
    /// </summary>
    /// <param name="configurator">A MassTransit endpoint configurator.</param>
    /// <param name="context">The message handling context, implementing <see cref="IConfigurationServiceProvider"/>.</param>
    /// <example>
    /// services.AddMassTransit(busConfigurator =>
    ///     {
    ///         busConfigurator.AddConsumer<MyHandler>();
    ///         busConfigurator.UsingRabbitMq((busContext, rabbitMQConfigurator) =>
    ///         {
    ///             rabbitMQConfigurator.ReceiveEndpoint(AssemblyName, endpointConfigurator =>
    ///             {
    ///                 endpointConfigurator.UseFluentValidationForMassTransit(busContext);
    ///                 endpointConfigurator.ConfigureConsumer<MyHandler>(busContext);
    ///             });
    ///         });
    ///     });
    /// </example>
    public static IEndpointConfigurator UseFluentValidationForMassTransit(this IEndpointConfigurator configurator, IBusRegistrationContext context)
    {
        configurator.UseConsumeFilter(typeof(FluentValidationFilter<>), context);
        return configurator;
    }
}