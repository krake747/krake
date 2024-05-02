using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.UpdatePortfolio;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static partial class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapUpdatePortfolio(this IEndpointRouteBuilder app)
    {
        app.MapPut("portfolios/{id:guid}", async ([FromRoute] Guid id, [FromBody] UpdatePortfolioRequest request,
                [FromServices] ISender mediatR, CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new UpdatePortfolioCommand(id, request.Name), token);
                return result.Match(ApiErrorMapping.MapToApiResult, _ => Results.NoContent());
            })
            .WithOpenApi()
            .Accepts<UpdatePortfolioRequest>(OpenApiSchemas.Accepts.Json)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent)
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName("UpdatePortfolio");

        return app;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class UpdatePortfolioRequest
{
    public required string Name { get; init; }
}