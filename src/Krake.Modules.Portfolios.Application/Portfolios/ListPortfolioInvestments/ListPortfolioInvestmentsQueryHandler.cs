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
             LEFT JOIN [Portfolios].[PortfolioInvestments] pi
                 ON p.[Id] = pi.[PortfolioId]
             LEFT JOIN [Portfolios].[Instruments] s
                 ON pi.[InstrumentId] = s.[Id]
             WHERE (p.[Id] = @PortfolioId OR @PortfolioId IS NULL)
             ORDER BY s.[Name] ASC
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