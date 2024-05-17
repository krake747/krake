using System.Data;
using Microsoft.Data.SqlClient;

namespace Krake.DatabaseMigrator.Data;

internal sealed class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}