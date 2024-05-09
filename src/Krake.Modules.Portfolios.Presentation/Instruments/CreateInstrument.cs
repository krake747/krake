using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Instruments.CreateInstrument;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Instruments;

internal static class CreateInstrument
{
    public static IEndpointRouteBuilder MapCreateInstrument(this IEndpointRouteBuilder app)
    {
        app.MapPost("instruments", async ([FromBody] CreateInstrumentRequest request, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var command = new CreateInstrumentCommand(
                    request.Name,
                    request.Currency,
                    request.Country,
                    request.Mic,
                    request.Sector,
                    request.Symbol,
                    request.Isin);

                var result = await mediatR.Send(command, token);
                return result.Match(
                    ApiErrorMapping.MapToApiResult,
                    id => Results.CreatedAtRoute("GetInstrument", new { id }, id));
            })
            .WithOpenApi()
            .Accepts<CreateInstrumentRequest>(OpenApiSchemas.Accepts.Json)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithTags(OpenApiSchemas.Tags.Instruments)
            .WithName("CreateInstrument");

        return app;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class CreateInstrumentRequest
{
    public required string Name { get; init; }
    public required string Currency { get; init; }
    public required string Country { get; init; }
    public required string Mic { get; init; }
    public required string Sector { get; init; }
    public required string Symbol { get; init; }
    public required string Isin { get; init; }
}