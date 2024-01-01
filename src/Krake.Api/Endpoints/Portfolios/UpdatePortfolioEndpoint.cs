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
            .Produces<PortfolioResponse>();

        return app;
    }

    private static async Task<IResult> UpdatePortfolioAsync(
        [FromServices] IPortfolioRepository portfolioRepository,
        [FromBody] UpdatePortfolioRequest request,
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var portfolio = await portfolioRepository.GetByIdAsync(id, token);
        var updatePortfolio = request.MapToUpdate(portfolio);
        var updated = await portfolioRepository.UpdateByIdAsync(id, updatePortfolio, token);
        return Results.Ok(new PortfolioResponse { Id = portfolio.Id, Name = updatePortfolio.Name });
    }
}