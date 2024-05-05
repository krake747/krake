using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Instruments.GetInstrument;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Instruments;

internal static class GetInstrument
{
    public static IEndpointRouteBuilder MapGetInstrument(this IEndpointRouteBuilder app)
    {
        app.MapGet("instruments/{id:guid}", async ([FromRoute] Guid id, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new GetInstrumentQuery(id), token);
                return result.Match(ApiErrorMapping.MapToApiResult, Results.Ok);
            })
            .WithOpenApi()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<InstrumentResponse>()
            .WithTags(OpenApiSchemas.Tags.Instruments)
            .WithName("GetInstrument");

        return app;
    }
}