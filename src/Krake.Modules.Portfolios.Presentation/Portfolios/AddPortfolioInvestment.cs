using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.AddPortfolioInvestment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static class AddPortfolioInvestment
{
    public static IEndpointRouteBuilder MapAddPortfolioInvestment(this IEndpointRouteBuilder app)
    {
        app.MapPost("portfolios/{id:guid}/investment", async (
                [FromRoute] Guid id,
                [FromBody] AddPortfolioInvestmentRequest request,
                [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var command = new AddPortfolioInvestmentCommand(
                    id,
                    request.InstrumentId,
                    request.PurchaseDate,
                    request.PurchasePrice,
                    request.Quantity);

                var result = await mediatR.Send(command, token);
                return result.Match(ApiErrorMapping.MapToApiResult, _ => Results.NoContent());
            })
            .WithOpenApi()
            .Accepts<CreatePortfolioRequest>(OpenApiSchemas.Accepts.Json)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<Guid>(StatusCodes.Status204NoContent)
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName(nameof(AddPortfolioInvestment));

        return app;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class AddPortfolioInvestmentRequest
{
    public required Guid InstrumentId { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public required decimal PurchasePrice { get; init; }
    public required decimal Quantity { get; init; }
}