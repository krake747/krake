using System.Data;
using System.Data.SqlClient;

namespace Krake.Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}