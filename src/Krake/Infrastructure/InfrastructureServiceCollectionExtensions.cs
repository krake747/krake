using Krake.Infrastructure.Database;
using Krake.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration config,
        string database)
    {
        services.AddDatabaseModule(config.GetConnectionString(database)!);
        services.AddEmailModule(config);
        return services;
    }
}