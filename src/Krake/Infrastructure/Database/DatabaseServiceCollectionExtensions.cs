using Microsoft.Extensions.DependencyInjection;

namespace Krake.Infrastructure.Database;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseModule(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        return services;
    }
}