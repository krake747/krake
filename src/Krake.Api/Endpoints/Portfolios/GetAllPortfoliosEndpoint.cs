using Krake.Application.Portfolios;
using Krake.Contracts.Portfolios.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Krake.Api.Endpoints.Portfolios;

public static class GetAllPortfoliosEndpoint
{
    private const string Name = "GetAllPortfolios";
    private const string Tags = "Portfolios";
    private const string Summary = "Get all portfolios";
    private const string Description = "Get all portfolios";

    public static IEndpointRouteBuilder MapGetAllPortfolios(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Portfolios.GetAll, GetAllProgrammersAsync)
            .WithName(Name)
            .WithTags(Tags)
            .WithSummary(Summary)
            .WithDescription(Description)
            .WithOpenApi()
            .Produces<PortfoliosResponse>();

        return app;
    }

    private static async Task<IResult> GetAllProgrammersAsync(
        [FromServices] IPortfolioRepository portfolioRepository,
        CancellationToken token = default)
    {
        var portfolios = await portfolioRepository.GetAllAsync(token);
        return Results.Ok(portfolios.MapToResponse());
    }
}