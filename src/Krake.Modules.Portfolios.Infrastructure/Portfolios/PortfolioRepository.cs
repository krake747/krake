using Dapper;
using Krake.Core.Application.Data;
using Krake.Modules.Portfolios.Application.Portfolios;
using Krake.Modules.Portfolios.Domain.Portfolios;
using Microsoft.Data.SqlClient;

namespace Krake.Modules.Portfolios.Infrastructure.Portfolios;

internal sealed class PortfolioRepository(IDbConnectionFactory connectionFactory) : IPortfolioRepository
{
    public async Task<bool> CreateAsync(Portfolio portfolio, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        const string sql =
            """
            INSERT INTO [Portfolios].[Portfolios] ([Id], [Name], [Currency])
            VALUES (@Id, @Name, @Currency)
            """;

        var command = new CommandDefinition(sql, portfolio, transaction, cancellationToken: token);
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

    public async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql =
            $"""
             SELECT TOP 1
                 [Id] AS [{nameof(Portfolio.Id)}],
                 [Name] AS [{nameof(Portfolio.Name)}],
                 [Currency] AS [{nameof(Portfolio.Currency)}]
             FROM [Portfolios].[Portfolios]
             WHERE [Id] = @Id
             """;

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);
        var portfolio = await connection.QuerySingleOrDefaultAsync<Portfolio>(command);
        return portfolio;
    }

    public async Task<IEnumerable<Portfolio>> ListAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT [Id], [Name], [Currency] FROM [Portfolios].[Portfolios]";
        var command = new CommandDefinition(sql, cancellationToken: token);
        var portfolios = await connection.QueryAsync<Portfolio>(command);
        return portfolios;
    }

    public async Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        const string sql = "UPDATE [Portfolios].[Portfolios] SET [Name] = @Name WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, portfolio, transaction, cancellationToken: token);
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

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        const string sql = "DELETE FROM [Portfolios].[Portfolios] WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: token);
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

    public async Task<bool> AddPortfolioInvestmentAsync(PortfolioInvestment investment, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        const string sql =
            """
            INSERT INTO [Portfolios].[PortfolioInvestments] ([PortfolioId], [InstrumentId], [PurchaseDate], [PurchasePrice], [Quantity])
            VALUES (@PortfolioId, @InstrumentId, @PurchaseDate, @PurchasePrice, @Quantity)
            """;

        var command = new CommandDefinition(sql, investment, transaction, cancellationToken: token);
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

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT COUNT(1) FROM [Portfolios].[Portfolios] WHERE [Id] = @Id;";
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);
        var success = await connection.ExecuteScalarAsync<int>(command);
        return success > 0;
    }
}