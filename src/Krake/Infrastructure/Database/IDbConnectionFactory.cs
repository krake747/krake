using System.Data;

namespace Krake.Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
    IDbConnection CreateConnection();
}