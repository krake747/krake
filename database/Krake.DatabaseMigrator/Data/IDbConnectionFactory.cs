using System.Data;

namespace Krake.DatabaseMigrator.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}