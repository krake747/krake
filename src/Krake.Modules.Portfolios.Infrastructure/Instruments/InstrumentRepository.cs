using Dapper;
using Krake.Core.Application.Data;
using Krake.Modules.Portfolios.Application.Instruments;
using Krake.Modules.Portfolios.Domain.Instruments;
using Microsoft.Data.SqlClient;

namespace Krake.Modules.Portfolios.Infrastructure.Instruments;

internal sealed class InstrumentRepository(IDbConnectionFactory connectionFactory) : IInstrumentRepository
{
    public async Task<bool> CreateAsync(Instrument instrument, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        const string sql =
            """
            INSERT INTO [Portfolios].[Instruments] ([Id], [Name], [Currency], [Country], [MIC], [Sector], [Symbol], [Isin])
            VALUES (@Id, @Name, @Currency, @Country, @Mic, @Sector, @Symbol, @Isin)
            """;

        var command = new CommandDefinition(sql, instrument, transaction, cancellationToken: token);
        try
        {
            var success = await connection.ExecuteAsync(command);
            transaction.Commit();
            return success > 0;
        }
        catch (SqlException)
        {
            transaction.Rollback();
            return false;
        }
    }

    public async Task<IEnumerable<Instrument>> ListAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        const string sql =
            $"""
             SELECT
             [Id] AS [{nameof(Instrument.Id)}],
             [Name] AS [{nameof(Instrument.Name)}],
             [Currency] AS [{nameof(Instrument.Currency)}],
             [Country] AS [{nameof(Instrument.Country)}],
             [Mic] AS [{nameof(Instrument.Mic)}],
             [Sector] AS [{nameof(Instrument.Sector)}],
             [Symbol] AS [{nameof(Instrument.Symbol)}],
             [Isin] AS [{nameof(Instrument.Isin)}]
             FROM [Portfolios].[Instruments]
             """;

        var command = new CommandDefinition(sql, cancellationToken: token);
        var instruments = await connection.QueryAsync<Instrument>(command);
        return instruments;
    }

    public async Task<Instrument?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        const string sql =
            $"""
             SELECT TOP 1
                 [Id] AS [{nameof(Instrument.Id)}],
                 [Name] AS [{nameof(Instrument.Name)}],
                 [Currency] AS [{nameof(Instrument.Currency)}],
                 [Country] AS [{nameof(Instrument.Country)}],
                 [Mic] AS [{nameof(Instrument.Mic)}],
                 [Sector] AS [{nameof(Instrument.Sector)}],
                 [Symbol] AS [{nameof(Instrument.Symbol)}],
                 [Isin] AS [{nameof(Instrument.Isin)}]
             FROM [Portfolios].[Instruments]
             WHERE [Id] = @Id
             """;

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);
        var instrument = await connection.QuerySingleOrDefaultAsync<Instrument>(command);
        return instrument;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT COUNT(1) FROM [Portfolios].[Instruments] WHERE [Id] = @Id;";
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);
        var success = await connection.ExecuteScalarAsync<int>(command);
        return success > 0;
    }
}