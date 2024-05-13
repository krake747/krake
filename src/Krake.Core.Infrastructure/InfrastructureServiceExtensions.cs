using Dapper;
using Krake.Core.Application.Caching;
using Krake.Core.Application.Data;
using Krake.Core.Infrastructure.Authentication;
using Krake.Core.Infrastructure.Caching;
using Krake.Core.Infrastructure.Data;
using Krake.Core.Infrastructure.Data.TypeHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Krake.Core.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string dbConnectionString, string redisConnectionString)
    {
        services.AddCoreAuthentication();

        services.AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory(dbConnectionString));
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

        services.TryAddSingleton<TimeProvider>(_ => TimeProvider.System);

        try
        {
            services.TryAddSingleton<ICacheService, RedisCacheService>();
            var connectionMultiplexer = (IConnectionMultiplexer)ConnectionMultiplexer.Connect(redisConnectionString);
            services.TryAddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(
                o => o.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch (RedisConnectionException)
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}