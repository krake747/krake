using Krake.Modules.Portfolios.Presentation.Portfolios;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace Krake.Modules.Portfolios.Presentation;

public static class PortfoliosModuleEndpoints
{
    public static IEndpointRouteBuilder MapPortfoliosModuleEndpoints(this IEndpointRouteBuilder app, ILogger logger)
    {
        app.MapPortfoliosEndpoints();

        logger.Information("{Module} module endpoints registered", "Portfolios");

        return app;
    }
}