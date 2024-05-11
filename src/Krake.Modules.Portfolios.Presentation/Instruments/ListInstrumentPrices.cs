using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Instruments.GetInstrumentPriceData;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Instruments;

internal static class ListInstrumentPrices
{
    public static IEndpointRouteBuilder MapListInstrumentPrices(this IEndpointRouteBuilder app)
    {
        app.MapGet("instruments/{id:guid}/prices", async ([FromRoute] Guid id, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new GetInstrumentPricesQuery(id), token);
                return result.Match(ApiErrorMapping.MapToApiResult, Results.Ok);
            })
            .WithOpenApi()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<InstrumentPricesResponse>()
            .WithTags(OpenApiSchemas.Tags.Instruments)
            .WithName(nameof(ListInstrumentPrices));

        return app;
    }
}