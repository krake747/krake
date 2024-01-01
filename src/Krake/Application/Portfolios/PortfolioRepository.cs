using Dapper;
using Krake.Infrastructure.Database;

namespace Krake.Application.Portfolios;

public interface IPortfolioRepository
{
    Task<Portfolio> GetByIdAsync(Guid portfolioId, CancellationToken token = default);
    Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default);
}

public sealed class PortfolioRepository(IDbConnectionFactory connectionFactory) : IPortfolioRepository
{
    public async Task<Portfolio> GetByIdAsync(Guid portfolioId, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT Id, Name FROM Portfolios WHERE Id = @Id";
        var command = new CommandDefinition(sql, new { Id = portfolioId }, cancellationToken: token);
        var portfolio = await connection.QuerySingleAsync<Portfolio>(command);
        return portfolio;
    }

    public async Task<IEnumerable<Portfolio>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        const string sql = "SELECT Id, Name FROM Portfolios";
        var command = new CommandDefinition(sql, cancellationToken: token);
        var portfolios = await connection.QueryAsync<Portfolio>(command);
        return portfolios;
    }
}