using System.Data;
using System.Reflection;
using System.Transactions;
using Krake.Core.Application.Data;
using Krake.Core.Extensions;
using Microsoft.Data.SqlClient;

namespace Krake.Core.Infrastructure.Data;

public static class SqlConnectionExtensions
{
    public static long BulkInsert<T>(IDbConnectionFactory connectionFactory, IEnumerable<T> data,
        string destinationTableName)
    {
        using var dataTable = CreateDataTable(data, destinationTableName);
        using var transactionScope = new TransactionScope();
        using var connection = connectionFactory.CreateConnection();
        using var copy = new SqlBulkCopy((SqlConnection)connection);

        copy.DestinationTableName = dataTable.TableName;
        copy.NotifyAfter = 1;

        var inserted = 0L;
        copy.SqlRowsCopied += (_, e) => inserted = e.RowsCopied;

        foreach (var column in dataTable.Columns)
        {
            var col = column.ToValueOrDefault();
            copy.ColumnMappings.Add(col, col);
        }

        copy.WriteToServer(dataTable);
        transactionScope.Complete();

        return inserted;
    }

    public static long BulkInsert<T>(IDbConnectionFactory connectionFactory, IEnumerable<T> data,
        Dictionary<string, string> columnMappings, string destinationTableName)
    {
        using var dataTable = CreateDataTable(data, destinationTableName);
        using var transactionScope = new TransactionScope();
        using var connection = connectionFactory.CreateConnection();
        using var copy = new SqlBulkCopy((SqlConnection)connection);

        copy.DestinationTableName = dataTable.TableName;
        copy.NotifyAfter = 1;

        var inserted = 0L;
        copy.SqlRowsCopied += (_, e) => inserted = e.RowsCopied;

        foreach (var column in dataTable.Columns)
        {
            var col = column.ToValueOrDefault();
            copy.ColumnMappings.Add(col, columnMappings.GetValueOrDefault(col, col));
        }

        copy.WriteToServer(dataTable);
        transactionScope.Complete();

        return inserted;
    }

    public static async Task<long> BulkInsertAsync<T>(IDbConnectionFactory connectionFactory, IEnumerable<T> data,
        Dictionary<string, string> columnMappings, string destinationTableName, CancellationToken token = default)
    {
        using var dataTable = CreateDataTable(data, destinationTableName);
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var copy = new SqlBulkCopy((SqlConnection)connection);

        var inserted = 0L;
        copy.DestinationTableName = dataTable.TableName;
        copy.NotifyAfter = 1;
        copy.SqlRowsCopied += (_, e) => inserted = e.RowsCopied;

        foreach (var column in dataTable.Columns)
        {
            var col = column.ToValueOrDefault();
            copy.ColumnMappings.Add(col, columnMappings.GetValueOrDefault(col, col));
        }

        await copy.WriteToServerAsync(dataTable, token);
        transactionScope.Complete();

        return inserted;
    }

    public static async Task<long> BulkInsertAsync<T>(IDbConnectionFactory connectionFactory, IEnumerable<T> data,
        string destinationTableName, CancellationToken token = default)
    {
        using var dataTable = CreateDataTable(data, destinationTableName);
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var copy = new SqlBulkCopy((SqlConnection)connection);

        var inserted = 0L;
        copy.DestinationTableName = dataTable.TableName;
        copy.NotifyAfter = 1;
        copy.SqlRowsCopied += (_, e) => inserted = e.RowsCopied;

        foreach (var column in dataTable.Columns)
        {
            var col = column.ToValueOrDefault();
            copy.ColumnMappings.Add(col, col);
        }

        await copy.WriteToServerAsync(dataTable, token);
        transactionScope.Complete();

        return inserted;
    }

    private static DataTable CreateDataTable<T>(IEnumerable<T> items, string tableName)
    {
        var dt = new DataTable(tableName);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in items)
        {
            var values = new object[props.Length];
            for (var i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null) ?? DBNull.Value;
            }

            dt.Rows.Add(values);
        }

        return dt;
    }
}