﻿using Krake.Core.Application.Caching;
using Krake.Core.Application.Data;
using Krake.Core.Infrastructure.Caching;
using Krake.Core.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Krake.Core.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure<TAssemblyMarker>(this IServiceCollection services,
        string dbConnectionString, string redisConnectionString)
    {
        services.AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory(dbConnectionString));
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