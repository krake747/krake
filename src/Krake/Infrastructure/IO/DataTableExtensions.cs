using System.Data;

namespace Krake.Infrastructure.IO;

public static class DataTableExtensions
{
    private static readonly Func<string, string, bool> Search = (x, v) => x.Equals(v, StringComparison.Ordinal);

    public static DataTable RemoveAbove(DataTable dt, string value, Func<string, string, bool>? search = default)
    {
        var (row, _) = FindCoordinates(dt, value, search ?? Search);
        return dt.AsEnumerable()
            .Skip(row)
            .CopyToDataTable();
    }

    public static DataTable RemoveBelow(DataTable dt, string value, Func<string, string, bool>? search = default)
    {
        var (row, _) = FindCoordinates(dt, value, search ?? Search);
        return dt.AsEnumerable()
            .Take(row + 1)
            .CopyToDataTable();
    }

    public static DataTable RemoveLeftOf(DataTable dt, string value, Func<string, string, bool>? search = default)
    {
        var copy = dt.Copy();
        var (_, col) = FindCoordinates(copy, value, search ?? Search);
        for (var i = col - 1; i >= 0; i--)
        {
            copy.Columns.RemoveAt(i);
        }

        return copy;
    }

    public static DataTable RemoveRightOf(DataTable dt, string value, Func<string, string, bool>? search = default)
    {
        var copy = dt.Copy();
        var (_, col) = FindCoordinates(copy, value, search ?? Search);
        for (var i = dt.Columns.Count - 1; i > col; i--)
        {
            copy.Columns.RemoveAt(i);
        }

        return copy;
    }

    public static DataTable RenameColumns(DataTable dt)
    {
        var copy = dt.Copy();
        var newHeaders = copy.Rows[0].ItemArray.Cast<string>().ToArray();
        for (var i = 0;  i < newHeaders.Length; i++)
        {
            copy.Columns[i].ColumnName = copy.Columns.Contains(newHeaders[i])
                ? IndexedHeader(newHeaders, copy.Columns, newHeaders[i])
                : newHeaders[i];
        }

        copy.Rows[0].Delete();
        copy.AcceptChanges();
        return copy;
    }

    public static DataTable Transpose(DataTable dt)
    {
        var transposed = new DataTable();
        for (var i = 0; i < dt.Rows.Count; i++)
        {
            _ = transposed.Columns.Add();
        }

        for (var col = 0; col < dt.Columns.Count; col++)
        {
            _ = transposed.Rows.Add();
            for (var row = 0; row < dt.Rows.Count; row++)
            {
                transposed.Rows[^1][row] = dt.Rows[row][col];
            }
        }

        return transposed;
    }

    private static (int Row, int Col) FindCoordinates(DataTable dt, string value, Func<string, string, bool> search)
    {
        for (var row = 0; row < dt.Rows.Count; row++)
        {
            for (var col = 0; col < dt.Columns.Count; col++)
            {
                if (search(dt.Rows[row][col].ToString() ?? string.Empty, value))
                {
                    return (row, col);
                }
            }
        }

        return (-1, -1);
    }

    private static string IndexedHeader(IEnumerable<string> columns, DataColumnCollection headers, string header)
    {
        var count = columns.Count(c => c.Contains(header));
        for (var i = 1; i < count; i++)
        {
            var indexedHeader = $"{header}_{i}";
            if (headers.Contains(indexedHeader) is false)
            {
                return indexedHeader;
            }
        }

        return header;
    }
}

