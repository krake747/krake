using Krake.Api.Endpoints.Portfolios;

namespace Krake.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPortfolioEndpoints();
        return app;
    }
}