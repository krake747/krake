using FluentValidation;
using Krake.Core.Application.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Core.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication<TAssemblyMarker>(this IServiceCollection services)
    {
        services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<TAssemblyMarker>();
            c.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehaviour<,>));
            c.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            c.AddOpenBehavior(typeof(ValidationPipelineBehaviour<,>));
        });

        services.AddValidatorsFromAssemblyContaining<TAssemblyMarker>(includeInternalTypes: true);

        return services;
    }
}