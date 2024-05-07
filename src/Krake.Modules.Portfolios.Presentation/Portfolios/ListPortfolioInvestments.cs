using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolioInvestments;
using Krake.Modules.Portfolios.Application.Portfolios.ListPortfolioInvestments;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static class ListPortfolioInvestments
{
    public static IEndpointRouteBuilder MapListPortfolioInvestments(this IEndpointRouteBuilder app)
    {
        app.MapGet("portfolios/investments", async ([FromQuery] Guid? portfolioId, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new ListPortfolioInvestmentsQuery(portfolioId), token);
                return result.Match(ApiErrorMapping.MapToApiResult, Results.Ok);
            })
            .WithOpenApi()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<IReadOnlyCollection<PortfolioInvestmentsResponse>>()
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName(nameof(ListPortfolioInvestments));

        return app;
    }
}