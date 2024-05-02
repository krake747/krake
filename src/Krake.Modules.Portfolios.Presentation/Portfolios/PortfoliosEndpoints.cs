using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static class PortfoliosEndpoints
{
    public static IEndpointRouteBuilder MapPortfoliosEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreatePortfolio();
        app.MapGetPortfolio();
        app.MapListPortfolios();
        app.MapUpdatePortfolio();
        app.MapDeletePortfolio();
        return app;
    }
}