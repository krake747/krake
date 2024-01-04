using Krake.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        return services;
    }
}