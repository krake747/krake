using Krake.Modules.Portfolios.Application.Instruments.GetInstrument;
using Krake.Modules.Portfolios.Application.Instruments.ListInstruments;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Instruments;

internal static class ListInstruments
{
    public static IEndpointRouteBuilder MapListInstruments(this IEndpointRouteBuilder app)
    {
        app.MapGet("instruments", async ([FromServices] ISender mediatR, CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new ListInstrumentsQuery(), token);
                return Results.Ok(result);
            })
            .WithOpenApi()
            .Produces<IReadOnlyCollection<InstrumentResponse>>()
            .WithTags(OpenApiSchemas.Tags.Instruments)
            .WithName("ListInstruments");

        return app;
    }
}