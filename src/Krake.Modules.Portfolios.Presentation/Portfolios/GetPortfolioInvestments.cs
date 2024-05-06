using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolioInvestments;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static class GetPortfolioInvestments
{
    public static IEndpointRouteBuilder MapGetPortfolioInvestments(this IEndpointRouteBuilder app)
    {
        app.MapGet("portfolios/{id:guid}/investments", async ([FromRoute] Guid id, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new GetPortfolioInvestmentsQuery(id), token);
                return result.Match(ApiErrorMapping.MapToApiResult, Results.Ok);
            })
            .WithOpenApi()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<PortfolioInvestmentsResponse>()
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName(nameof(GetPortfolioInvestments));

        return app;
    }
}