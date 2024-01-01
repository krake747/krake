using Krake.Api.Mapping;
using Krake.Application.Portfolios;
using Krake.Contracts.Portfolios.Requests;
using Krake.Contracts.Portfolios.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Krake.Api.Endpoints.Portfolios;

public static class UpdatePortfolioEndpoint
{
    private const string Name = "UpdatePortfolio";
    private const string Tags = "Portfolios";
    private const string Summary = "Update portfolio";
    private const string Description = "Update portfolio by id (Guid)";
    private const string ContentType = "application/json";

    public static IEndpointRouteBuilder MapUpdatePortfolio(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Portfolios.Update, UpdatePortfolioAsync)
            .WithName(Name)
            .WithTags(Tags)
            .WithSummary(Summary)
            .WithDescription(Description)
            .WithOpenApi()
            .Accepts<UpdatePortfolioRequest>(ContentType)
            .Produces<PortfolioResponse>()
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> UpdatePortfolioAsync(
        [FromServices] IPortfolioService portfolioService,
        [FromBody] UpdatePortfolioRequest request,
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var updatePortfolio = new UpdatePortfolio(request.Name);
        var portfolioResult = await portfolioService.UpdateByIdAsync(id, updatePortfolio, token);
        return portfolioResult.Match(
            error => Results.NotFound(error.MapToResponse()),
            portfolio => Results.Ok(portfolio.MapToResponse()));
    }
}