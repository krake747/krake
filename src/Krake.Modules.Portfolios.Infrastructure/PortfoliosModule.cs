using Krake.Core.Application;
using Krake.Core.Infrastructure;
using Krake.Modules.Portfolios.Application;
using Krake.Modules.Portfolios.Application.Portfolios;
using Krake.Modules.Portfolios.Infrastructure.Portfolios;
using Krake.Modules.Portfolios.Presentation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Modules.Portfolios.Infrastructure;

public static class PortfoliosModule
{
    public static IEndpointRouteBuilder MapPortfoliosModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPortfoliosApiEndpoints();
        return app;
    }

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