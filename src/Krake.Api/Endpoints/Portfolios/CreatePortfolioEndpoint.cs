using Krake.Api.Mapping;
using Krake.Application.Portfolios;
using Krake.Contracts.Errors.Responses;
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
            .Produces<PortfolioResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreatePortfolioAsync(
        [FromServices] IPortfolioService portfolioService,
        [FromBody] CreatePortfolioRequest request,
        CancellationToken token = default)
    {
        var createPortfolio = new CreatePortfolio(request.Name);
        var createdResult = await portfolioService.CreateAsync(createPortfolio, token);
        return createdResult.Match(
            errors => Results.BadRequest(errors.MapToResponse()),
            portfolio => Results.CreatedAtRoute(GetPortfolioEndpoint.Name, new { id = portfolio.Id }, 
                portfolio.MapToResponse()));
    }
}