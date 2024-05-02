using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.DeletePortfolio;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static partial class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapDeletePortfolio(this IEndpointRouteBuilder app)
    {
        app.MapDelete("portfolios/{id:guid}", async ([FromRoute] Guid id, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new DeletePortfolioCommand(id), token);
                return result.Match(ApiErrorMapping.MapToApiResult, _ => Results.NoContent());
            })
            .WithOpenApi()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent)
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName("DeletePortfolio");

        return app;
    }
}