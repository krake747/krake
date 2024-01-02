using Krake.Api.Mapping;
using Krake.Application.Portfolios;
using Krake.Contracts.Errors.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Krake.Api.Endpoints.Portfolios;

public static class DeletePortfolioEndpoint
{
    private const string Name = "DeletePortfolio";
    private const string Tags = "Portfolios";
    private const string Summary = "Delete portfolio";
    private const string Description = "Delete an existing portfolio";

    public static IEndpointRouteBuilder MapDeletePortfolio(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Portfolios.Delete, DeletePortfolioAsync)
            .WithName(Name)
            .WithTags(Tags)
            .WithSummary(Summary)
            .WithDescription(Description)
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> DeletePortfolioAsync(
        [FromServices] IPortfolioService portfolioService,
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var deleteResult = await portfolioService.DeleteByIdAsync(id, token);
        return deleteResult.Match(
            error => Results.NotFound(error.MapToResponse()),
            _ => Results.NoContent());
    }
}