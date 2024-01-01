using System.Data.SqlClient;
using Dapper;
using Krake.Core;
using Krake.Infrastructure.Database;

namespace Krake.Application.Portfolios;

public interface IPortfolioRepository
{
    Task<Result<Error, Created>> CreateAsync(Portfolio portfolio, CancellationToken token = default);
    Task<Result<Error, Portfolio>> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default);
    Task<Result<Error, Updated>> UpdateAsync(Portfolio portfolio, CancellationToken token = default);
    Task<Result<Error, Deleted>> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<Error, Success>> ExistsByIdAsync(Guid id, CancellationToken token = default);
}

public sealed class PortfolioRepository(IDbConnectionFactory connectionFactory) : IPortfolioRepository
{
    public async Task<Result<Error, Created>> CreateAsync(Portfolio portfolio, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        const string sql = "INSERT INTO Portfolios ([Id], [Name]) VALUES (@Id, @Name)";
        var command = new CommandDefinition(sql, portfolio, transaction, cancellationToken: token);
        try
        {
            var success = await connection.ExecuteAsync(command);
            transaction.Commit();
            return success is 0 ? Error.Unexpected() : Ok.Created;
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return Error.Failure(ex.Message);
        }
    }

    public async Task<Result<Error, Portfolio>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT TOP 1 [Id], [Name] FROM Portfolios WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);
        var portfolio = await connection.QuerySingleOrDefaultAsync<Portfolio>(command);
        return portfolio is null ? Error.NotFound() : portfolio;
    }

    public async Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT [Id], [Name] FROM Portfolios";
        var command = new CommandDefinition(sql, cancellationToken: token);
        var portfolios = await connection.QueryAsync<Portfolio>(command);
        return portfolios;
    }
    
    public async Task<Result<Error, Updated>> UpdateAsync(Portfolio portfolio, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        const string sql = "UPDATE Portfolios SET [Name] = @Name WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, portfolio, transaction, cancellationToken: token);
        try
        {
            var success = await connection.ExecuteAsync(command);
            transaction.Commit();
            return success is 0 ? Error.Unexpected() : Ok.Updated;
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return Error.Failure(ex.Message);
        }
    }

    public async Task<Result<Error, Deleted>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        const string sql = "DELETE FROM Portfolios WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: token);
        try
        {
            var success = await connection.ExecuteAsync(command);
            transaction.Commit();
            return success is 0 ? Error.Unexpected() : Ok.Deleted;
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            return Error.Failure(ex.Message);
        }
    }
    
    public async Task<Result<Error, Success>> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT COUNT(1) FROM Portfolios WHERE [Id] = @Id;";
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);
        var success = await connection.ExecuteScalarAsync<int>(command);
        return success is 0 ? Error.NotFound() : Ok.Success;
    }
}