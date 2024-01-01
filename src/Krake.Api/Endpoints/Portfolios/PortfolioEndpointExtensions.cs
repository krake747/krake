namespace Krake.Api.Endpoints.Portfolios;

public static class PortfolioEndpointExtensions
{
    public static IEndpointRouteBuilder MapPortfolioEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetPortfolio();
        app.MapGetAllPortfolios();
        return app;
    }
}