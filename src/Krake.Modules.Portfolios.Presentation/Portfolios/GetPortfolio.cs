using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static partial class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapGetPortfolio(this IEndpointRouteBuilder app)
    {
        app.MapGet("portfolios/{id:guid}", async ([FromRoute] Guid id, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new GetPortfolioQuery(id), token);
                return result.Match(ApiErrorMapping.MapToApiResult, Results.Ok);
            })
            .WithOpenApi()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<PortfolioResponse>()
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName("GetPortfolio");

        return app;
    }
}