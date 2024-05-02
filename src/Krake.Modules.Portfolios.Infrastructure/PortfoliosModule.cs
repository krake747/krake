using Krake.Core.Application;
using Krake.Core.Infrastructure;
using Krake.Modules.Portfolios.Application;
using Krake.Modules.Portfolios.Application.Portfolios;
using Krake.Modules.Portfolios.Infrastructure.Portfolios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Modules.Portfolios.Infrastructure;

public static class PortfoliosModule
{
    public static IServiceCollection AddPortfoliosModule(this IServiceCollection services, IConfiguration config)
    {
        var dbConnectionString = config.GetConnectionString("SqlDatabase")!;
        var redisConnectionString = config.GetConnectionString("RedisCache")!;

        services.AddApplication<IPortfoliosApplicationMarker>();
        services.AddInfrastructure<IPortfoliosInfrastructureMarker>(dbConnectionString, redisConnectionString);

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();

        return services;
    }
}