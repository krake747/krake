using Krake.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Infrastructure;

public static class InfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
        return services;
    }
}