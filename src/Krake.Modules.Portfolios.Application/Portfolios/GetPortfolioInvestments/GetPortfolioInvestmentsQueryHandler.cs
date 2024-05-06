using Dapper;
using Krake.Core.Application.Data;
using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Domain.Portfolios;

namespace Krake.Modules.Portfolios.Application.Portfolios.GetPortfolioInvestments;

public sealed record GetPortfolioInvestmentsQuery(Guid PortfolioId)
    : IQuery<ErrorBase, PortfolioInvestmentsResponse>;

internal sealed class GetPortfolioInvestmentsQueryHandler(
    IDbConnectionFactory connectionFactory,
    IReadOnlyPortfolioRepository portfolioRepository)
    : IQueryHandler<GetPortfolioInvestmentsQuery, ErrorBase, PortfolioInvestmentsResponse>
{
    public async Task<Result<ErrorBase, PortfolioInvestmentsResponse>> Handle(GetPortfolioInvestmentsQuery request,
        CancellationToken token = default)
    {
        var exists = await portfolioRepository.ExistsByIdAsync(request.PortfolioId, token);
        if (exists is false)
        {
            return PortfolioErrors.NotFound(request.PortfolioId);
        }

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        const string sql =
            $"""
             SELECT
                 p.[Id] AS [{nameof(PortfolioInvestmentsResponse.Id)}],
                 p.[Name] AS [{nameof(PortfolioInvestmentsResponse.Name)}],
                 p.[Currency] AS [{nameof(PortfolioInvestmentsResponse.Currency)}],
                 s.[Id] AS [{nameof(PortfolioInvestmentResponse.InstrumentId)}],
                 s.[Name] AS [{nameof(PortfolioInvestmentResponse.InstrumentName)}],
                 s.[Currency] AS [{nameof(PortfolioInvestmentResponse.InstrumentCurrency)}],
                 pi.[PurchaseDate] AS [{nameof(PortfolioInvestmentResponse.PurchaseDate)}],
                 pi.[PurchasePrice] AS [{nameof(PortfolioInvestmentResponse.PurchasePrice)}],
                 pi.[Quantity] AS [{nameof(PortfolioInvestmentResponse.Quantity)}]
             FROM [Portfolios].[Portfolios] p
             JOIN [Portfolios].[PortfolioInvestments] pi
                 ON p.[Id] = pi.[PortfolioId]
             JOIN [Portfolios].[Instruments] s
                 ON pi.[InstrumentId] = s.[Id]
             WHERE p.[Id] = @PortfolioId
             ORDER BY s.[Name] ASC
             """;

        var portfolios = new Dictionary<Guid, PortfolioInvestmentsResponse>();
        _ = await connection
            .QueryAsync<PortfolioInvestmentsResponse, PortfolioInvestmentResponse, PortfolioInvestmentsResponse>(
                new CommandDefinition(sql, request, cancellationToken: token),
                (portfolio, investment) =>
                {
                    if (portfolios.TryAdd(portfolio.Id, portfolio))
                    {
                        portfolio.Investments.Add(investment);
                        return portfolio;
                    }

                    portfolios[portfolio.Id].Investments.Add(investment);
                    return portfolio;
                },
                nameof(PortfolioInvestmentResponse.InstrumentId));

        return portfolios[request.PortfolioId];
    }
}