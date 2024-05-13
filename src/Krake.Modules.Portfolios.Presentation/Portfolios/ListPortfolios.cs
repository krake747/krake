using Krake.Core.Application.Caching;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolio;
using Krake.Modules.Portfolios.Application.Portfolios.ListPortfolios;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static partial class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapListPortfolios(this IEndpointRouteBuilder app)
    {
        app.MapGet("portfolios", async ([FromServices] ISender mediatR, [FromServices] ICacheService cache,
                CancellationToken token = default) =>
            {
                var portfoliosResponse =
                    await cache.GetAsync<IReadOnlyCollection<PortfolioResponse>>("portfolios", token);

                if (portfoliosResponse is not null)
                {
                    return Results.Ok(portfoliosResponse);
                }

                var result = await mediatR.Send(new ListPortfoliosQuery(), token);
                await cache.SetAsync("portfolios", result, TimeSpan.FromSeconds(5), token);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithOpenApi()
            .Produces<IReadOnlyCollection<PortfolioResponse>>()
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName("ListPortfolios");

        return app;
    }
}