using System.Data;

namespace Krake.Migrator.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}