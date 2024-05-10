using Dapper;
using Krake.Core.Application.Data;
using Krake.Core.Application.Messaging;
using Krake.Core.Monads;
using Krake.Modules.Portfolios.Application.Portfolios.GetPortfolioInvestments;

namespace Krake.Modules.Portfolios.Application.Portfolios.ListPortfolioInvestments;

public sealed record ListPortfolioInvestmentsQuery(Guid? PortfolioId)
    : IQuery<ErrorBase, IReadOnlyCollection<PortfolioInvestmentsResponse>>;

internal sealed class ListPortfolioInvestmentsQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ListPortfolioInvestmentsQuery, ErrorBase, IReadOnlyCollection<PortfolioInvestmentsResponse>>
{
    public async Task<Result<ErrorBase, IReadOnlyCollection<PortfolioInvestmentsResponse>>> Handle(
        ListPortfolioInvestmentsQuery request, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        const string sql = //lang=sql
            $"""
             SELECT
                 p.[Id] AS [{nameof(PortfolioInvestmentsResponse.Id)}],
                 p.[Name] AS [{nameof(PortfolioInvestmentsResponse.Name)}],
                 p.[Currency] AS [{nameof(PortfolioInvestmentsResponse.Currency)}],
                 CAST(ISNULL(SUM(pi.[PurchasePrice] * pi.[Quantity]) OVER(PARTITION BY p.[Id]), 0) AS decimal(19,2)) AS [{nameof(PortfolioInvestmentsResponse.CostValue)}],
                 CAST(ISNULL(SUM(latest.[Price] * pi.[Quantity]) OVER(PARTITION BY p.[Id]), 0) AS decimal(19,2)) AS [{nameof(PortfolioInvestmentsResponse.TotalValue)}],
                 i.[Id] AS [{nameof(PortfolioInvestmentResponse.InstrumentId)}],
                 i.[Name] AS [{nameof(PortfolioInvestmentResponse.InstrumentName)}],
                 i.[Currency] AS [{nameof(PortfolioInvestmentResponse.InstrumentCurrency)}],
                 i.[Country] AS [{nameof(PortfolioInvestmentResponse.InstrumentCountry)}],
                 i.[Mic] AS [{nameof(PortfolioInvestmentResponse.InstrumentMic)}],
                 i.[Sector] AS [{nameof(PortfolioInvestmentResponse.InstrumentSector)}],
                 i.[Symbol] AS [{nameof(PortfolioInvestmentResponse.InstrumentSymbol)}],
                 i.[Isin] AS [{nameof(PortfolioInvestmentResponse.InstrumentIsin)}],
                 pi.[PurchaseDate] AS [{nameof(PortfolioInvestmentResponse.PurchaseDate)}],
                 pi.[PurchasePrice] AS [{nameof(PortfolioInvestmentResponse.PurchasePrice)}],
                 pi.[Quantity] AS [{nameof(PortfolioInvestmentResponse.Quantity)}],
                 latest.[Date] AS [{nameof(PortfolioInvestmentResponse.LatestDate)}],
                 latest.[Price] AS [{nameof(PortfolioInvestmentResponse.LatestPrice)}],
                 CAST((latest.[Price] - pi.[PurchasePrice]) *  pi.[Quantity] AS decimal(19,2)) AS [{nameof(PortfolioInvestmentResponse.TotalGain)}],
                 CAST((latest.[Price] / pi.[PurchasePrice] - 1) AS decimal(19,4)) AS [{nameof(PortfolioInvestmentResponse.PercentageGain)}]
             FROM [Portfolios].[Portfolios] p
             LEFT JOIN [Portfolios].[PortfolioInvestments] pi
                 ON p.[Id] = pi.[PortfolioId]
             LEFT JOIN [Portfolios].[Instruments] i
                 ON pi.[InstrumentId] = i.[Id]
             OUTER APPLY (
             	SELECT TOP 1
             		ipd.[Date],
             		ipd.[Close] AS [Price]
             	FROM [Portfolios].[InstrumentsPriceData] ipd
             	WHERE ipd.[InstrumentId] = pi.[InstrumentId]
             	ORDER BY ipd.[Date] DESC
             ) latest
             WHERE (p.[Id] = @PortfolioId OR @PortfolioId IS NULL)
             ORDER BY p.[Name], i.[Name] ASC
             """;

        var portfolios = new Dictionary<Guid, PortfolioInvestmentsResponse>();
        _ = await connection
            .QueryAsync<PortfolioInvestmentsResponse, PortfolioInvestmentResponse?, PortfolioInvestmentsResponse>(
                new CommandDefinition(sql, request, cancellationToken: token),
                (portfolio, investment) =>
                {
                    _ = portfolios.TryAdd(portfolio.Id, portfolio);
                    if (investment is not null)
                    {
                        portfolios[portfolio.Id].Investments.Add(investment);
                    }

                    return portfolio;
                },
                nameof(PortfolioInvestmentResponse.InstrumentId));

        return portfolios.Values;
    }
}