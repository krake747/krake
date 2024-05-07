using Krake.Modules.Portfolios.Presentation.Instruments;
using Krake.Modules.Portfolios.Presentation.Portfolios;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation;

public static class PortfoliosModuleEndpoints
{
    public static IEndpointRouteBuilder MapPortfoliosEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreatePortfolio();
        app.MapGetPortfolio();
        app.MapListPortfolios();
        app.MapUpdatePortfolio();
        app.MapDeletePortfolio();
        app.MapAddPortfolioInvestment();
        app.MapGetPortfolioInvestments();
        app.MapListPortfolioInvestments();

        app.MapCreateInstrument();
        app.MapGetInstrument();
        app.MapListInstruments();

        return app;
    }
}