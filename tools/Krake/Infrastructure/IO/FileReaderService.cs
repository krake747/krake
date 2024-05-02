using System.Data;

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
}