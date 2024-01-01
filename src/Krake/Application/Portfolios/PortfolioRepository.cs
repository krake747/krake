using Dapper;
using Krake.Infrastructure.Database;

namespace Krake.Application.Portfolios;

public interface IPortfolioRepository
{
    Task<bool> CreateAsync(CreatePortfolio createPortfolio, CancellationToken token = default);
    Task<Portfolio> GetByIdAsync(Guid portfolioId, CancellationToken token = default);
    Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default);
    Task<bool> UpdateByIdAsync(Guid portfolioId, UpdatePortfolio updatePortfolio, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid portfolioId, CancellationToken token = default);
}

public sealed class PortfolioRepository(IDbConnectionFactory connectionFactory) : IPortfolioRepository
{
    public async Task<bool> CreateAsync(CreatePortfolio createPortfolio, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "INSERT INTO Portfolios ([Name]) VALUES (@Name)";
        var command = new CommandDefinition(sql, createPortfolio, cancellationToken: token);
        var created = await connection.ExecuteAsync(command);
        return created > 0;
    }

    public async Task<Portfolio> GetByIdAsync(Guid portfolioId, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT [Id], [Name] FROM Portfolios WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, new { Id = portfolioId }, cancellationToken: token);
        var portfolio = await connection.QuerySingleOrDefaultAsync<Portfolio>(command);
        return portfolio!;
    }

    public async Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT [Id], [Name] FROM Portfolios";
        var command = new CommandDefinition(sql, cancellationToken: token);
        var portfolios = await connection.QueryAsync<Portfolio>(command);
        return portfolios;
    }

    public async Task<bool> UpdateByIdAsync(Guid portfolioId, UpdatePortfolio updatePortfolio, 
        CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "UPDATE Portfolios SET [Name] = @Name WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, updatePortfolio, cancellationToken: token);
        var updated = await connection.ExecuteAsync(command);
        return updated > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid portfolioId, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "DELETE FROM Portfolios WHERE [Id] = @Id";
        var command = new CommandDefinition(sql, new { Id = portfolioId }, cancellationToken: token);
        var deleted = await connection.ExecuteAsync(command);
        return deleted > 0;
    }
}