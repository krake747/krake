using Krake.Core.Application;
using Krake.Core.Infrastructure;
using Krake.Modules.Portfolios.Application;
using Krake.Modules.Portfolios.Application.Portfolios;
using Krake.Modules.Portfolios.Infrastructure.Portfolios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Krake.Modules.Portfolios.Infrastructure;

public static class PortfoliosModule
{
    public static IServiceCollection AddPortfoliosModule(this IServiceCollection services, IConfiguration config,
        ILogger logger)
    {
        var dbConnectionString = config.GetConnectionString("SqlDatabase")!;
        var redisConnectionString = config.GetConnectionString("RedisCache")!;

        services.AddApplication<IPortfoliosApplicationMarker>();
        services.AddInfrastructure<IPortfoliosInfrastructureMarker>(dbConnectionString, redisConnectionString);

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();

        logger.Information("{Module} module services registered", "Portfolios");

        return services;
    }
}