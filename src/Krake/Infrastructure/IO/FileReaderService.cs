using System.Data;
using ClosedXML.Excel;
using Krake.Infrastructure.IO.Models;

namespace Krake.Infrastructure.IO;

public static class FileReaderService
{
 public static IEnumerable<Dictionary<string, string>> ReadCsvFile(FileReaderOptions options, FileInfo file)
    {
        var allLines = File.ReadAllLines(file.FullName);
        var headers = allLines[0].Split(options.Delimiter);
        return allLines[1..]
            .Select(line => headers.Zip(line.Split(options.Delimiter), KeyValuePair.Create)
                .Where(x => string.IsNullOrWhiteSpace(x.Key) is false)
                .ToDictionary());
    }

    public static DataTable ReadCsvFileAsDataTable(FileReaderOptions options, FileInfo file)
    {
        using var sr = new StreamReader(file.FullName);
        var dt = new DataTable();
        var str = sr.ReadLine() ?? string.Empty;
        var headers = str.Split(options.Delimiter);

        foreach (var header in headers)
        {
            dt.Columns.Add(header);
        }

        while (sr.EndOfStream is false)
        {
            var data = sr.ReadLine() ?? string.Empty;
            var rows = data.Split(options.Delimiter);
            var dr = dt.NewRow();
            for (var i = 0; i < headers.Length; i++)
            {
                dr[i] = rows[i];
            }

            dt.Rows.Add(dr);
        }

        return dt;
    }

    public static DataSet ReadExcelFile(ExcelFileReaderOptions options, FileInfo file)
    {
        using var workbook = new XLWorkbook(file.FullName);
        var ds = new DataSet();
        foreach (var worksheet in workbook.Worksheets)
        {
            var dt = WorksheetToDataTable(options, worksheet);
            ds.Tables.Add(dt);
        }

        return ds;
    }

    public static DataTable ReadFirstExcelSheet(ExcelFileReaderOptions options, FileInfo file) =>
        ReadExcelFile(options, file).Tables[0];

    private static DataTable WorksheetToDataTable(ExcelFileReaderOptions options, IXLWorksheet worksheet)
    {
        var dt = new DataTable(worksheet.Name);
        var rows = options.SkipEmptyRows
            ? worksheet.Rows().Where(r => r.IsEmpty() is false).ToArray()
            : [.. worksheet.Rows()];

        if (options.HasHeaders)
        {
            var cells = rows[0].Cells();
            foreach (var cell in cells)
            {
                var header = cell.Value.ToString();
                var headers = dt.Columns;
                _ = dt.Columns.Add(headers.Contains(header) ? IndexedHeader(cells, headers, header) : header);
            }
        }

        var startIndex = options.HasHeaders is false ? 0 : 1;
        foreach (var row in rows[startIndex..])
        {
            var cellsUsed = row.CellsUsed().Count();
            if (cellsUsed is 0)
            {
                continue;
            }

            _ = dt.Rows.Add();
            var cells = row.Cells(usedCellsOnly: false);
            var cellsCount = cells.Count();
            foreach (var cell in cells)
            {
                var maxColumnNumber = Math.Max(cellsCount, cell.Address.ColumnNumber);
                for (var i = dt.Columns.Count; i < maxColumnNumber; i++)
                {
                    _ = dt.Columns.Add();
                }

                var columnIndex = cell.Address.ColumnNumber - 1;
                var value = cell.CachedValue.ToString();
                dt.Rows[^1][columnIndex] = value;
            }
        }

        return dt;
    }

    private static string IndexedHeader(IXLCells cells, DataColumnCollection headers, string header)
    {
        var count = cells.Count(c => c.CachedValue.ToString().Contains(header));
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