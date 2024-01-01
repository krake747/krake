using Krake.Application.Portfolios;
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
            .Produces(StatusCodes.Status204NoContent);
        
        return app;
    }

    private static async Task<IResult> DeletePortfolioAsync(
        [FromServices] IPortfolioRepository portfolioRepository,
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var deleted = await portfolioRepository.DeleteByIdAsync(id, token);
        return Results.NoContent();
    }
}