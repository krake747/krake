using System.Data;

namespace Krake.Core.Application.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
    IDbConnection CreateConnection();
}