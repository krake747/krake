using Krake.Application.Portfolios;
using Krake.Contracts.Portfolios.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Krake.Api.Endpoints.Portfolios;

public static class GetPortfolioEndpoint
{
    internal const string Name = "GetPortfolio";
    private const string Tags = "Portfolios";
    private const string Summary = "Get portfolio";
    private const string Description = "Get portfolio by id (Guid)";

    public static IEndpointRouteBuilder MapGetPortfolio(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Portfolios.Get, GetPortfolioAsync)
            .WithName(Name)
            .WithTags(Tags)
            .WithSummary(Summary)
            .WithDescription(Description)
            .WithOpenApi()
            .Produces<PortfolioResponse>();

        return app;
    }

    private static async Task<IResult> GetPortfolioAsync(
        [FromServices] IPortfolioRepository portfolioRepository,
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var portfolio = await portfolioRepository.GetByIdAsync(id, token);
        return Results.Ok(portfolio.MapToResponse());
    }
}