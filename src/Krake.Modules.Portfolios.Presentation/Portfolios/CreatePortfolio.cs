using Krake.Core.Presentation.Mapping;
using Krake.Modules.Portfolios.Application.Portfolios.CreatePortfolio;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Krake.Modules.Portfolios.Presentation.Portfolios;

internal static partial class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapCreatePortfolio(this IEndpointRouteBuilder app)
    {
        app.MapPost("portfolios", async ([FromBody] CreatePortfolioRequest request, [FromServices] ISender mediatR,
                CancellationToken token = default) =>
            {
                var result = await mediatR.Send(new CreatePortfolioCommand(request.Name, request.Currency), token);
                return result.Match(
                    ApiErrorMapping.MapToApiResult,
                    id => Results.CreatedAtRoute("GetPortfolio", new { id }, id));
            })
            .WithOpenApi()
            .Accepts<CreatePortfolioCommand>(OpenApiSchemas.Accepts.Json)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithTags(OpenApiSchemas.Tags.Portfolios)
            .WithName("CreatePortfolio");

        return app;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class CreatePortfolioRequest
{
    public required string Name { get; init; }
    public required string Currency { get; init; }
}