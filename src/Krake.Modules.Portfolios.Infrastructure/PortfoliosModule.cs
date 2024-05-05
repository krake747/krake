using Krake.Core.Application;
using Krake.Core.Infrastructure;
using Krake.Modules.Portfolios.Application;
using Krake.Modules.Portfolios.Application.Instruments;
using Krake.Modules.Portfolios.Application.Portfolios;
using Krake.Modules.Portfolios.Infrastructure.Instruments;
using Krake.Modules.Portfolios.Infrastructure.Portfolios;
using Krake.Modules.Portfolios.Presentation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Krake.Modules.Portfolios.Infrastructure;

public static class PortfoliosModule
{
    public static IEndpointRouteBuilder MapPortfoliosModuleEndpoints(this IEndpointRouteBuilder app, ILogger logger)
    {
        app.MapPortfoliosEndpoints();

        logger.Information("{Module} module API endpoints registered", "Portfolios");

        return app;
    }

    public static IServiceCollection AddPortfoliosModule(this IServiceCollection services, IConfiguration config,
        ILogger logger)
    {
        var dbConnectionString = config.GetConnectionString("SqlDatabase")!;
        var redisConnectionString = config.GetConnectionString("RedisCache")!;

        services.AddApplication<IPortfoliosApplicationMarker>();
        services.AddInfrastructure<IPortfoliosInfrastructureMarker>(dbConnectionString, redisConnectionString);

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IReadOnlyPortfolioRepository>(sp => sp.GetRequiredService<IPortfolioRepository>());
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<IReadOnlyInstrumentRepository>(sp => sp.GetRequiredService<IInstrumentRepository>());

        logger.Information("{Module} module services registered", "Portfolios");

        return services;
    }
}