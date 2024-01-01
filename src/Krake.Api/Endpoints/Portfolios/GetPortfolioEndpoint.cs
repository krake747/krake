using Krake.Api.Mapping;
using Krake.Application.Portfolios;
using Krake.Contracts.Errors.Responses;
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
            .Produces<PortfolioResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetPortfolioAsync(
        [FromServices] IPortfolioService portfolioService,
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var portfolioResult = await portfolioService.GetByIdAsync(id, token);
        return portfolioResult.Match(
            error => Results.NotFound(error.MapToResponse()),
            portfolio => Results.Ok(portfolio.MapToResponse()));
    }
}