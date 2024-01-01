using Krake.Application.Portfolios;
using Krake.Contracts.Portfolios.Requests;
using Krake.Contracts.Portfolios.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Krake.Api.Endpoints.Portfolios;

public static class CreatePortfolioEndpoint
{
    private const string Name = "CreatePortfolio";
    private const string Tags = "Portfolios";
    private const string Summary = "Create portfolio";
    private const string Description = "Create a new portfolio";
    private const string ContentType = "application/json";

    public static IEndpointRouteBuilder MapCreatePortfolio(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Portfolios.Create, CreatePortfolioAsync)
            .WithName(Name)
            .WithTags(Tags)
            .WithSummary(Summary)
            .WithDescription(Description)
            .WithOpenApi()
            .Accepts<CreatePortfolioRequest>(ContentType)
            .Produces<PortfolioResponse>();

        return app;
    }

    private static async Task<IResult> CreatePortfolioAsync(
        [FromServices] IPortfolioRepository portfolioRepository,
        [FromBody] CreatePortfolioRequest request,
        CancellationToken token = default)
    {
        var createPortfolio = request.MapToCreate(Guid.NewGuid());
        var created = await portfolioRepository.CreateAsync(createPortfolio, token);
        return Results.CreatedAtRoute(GetPortfolioEndpoint.Name, new { id = createPortfolio.Id },
            createPortfolio.MapToResponse());
    }
}