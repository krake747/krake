using Krake.Modules.Portfolios.Presentation.Portfolios;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation;

public static class PortfoliosModuleEndpoints
{
    public static IEndpointRouteBuilder MapPortfoliosModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPortfoliosEndpoints();
        return app;
    }
}