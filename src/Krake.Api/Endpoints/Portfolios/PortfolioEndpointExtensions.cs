namespace Krake.Api.Endpoints.Portfolios;

public static class PortfolioEndpointExtensions
{
    public static IEndpointRouteBuilder MapPortfolioEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreatePortfolio();
        app.MapGetPortfolio();
        app.MapGetAllPortfolios();
        app.MapUpdatePortfolio();
        app.MapDeletePortfolio();
        return app;
    }
}