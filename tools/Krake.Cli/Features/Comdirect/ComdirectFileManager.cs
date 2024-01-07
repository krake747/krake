using System.Text;
using Krake.Core;
using Krake.Infrastructure.IO.Common;
using Krake.Infrastructure.IO.Models;

namespace Krake.Cli.Features.Comdirect;

public sealed class ComdirectFileManager
    : IFileReaderService, IFileWriterService, IFileExporterService
{
    public Result<Error, Success> Export(IReadOnlyList<string> data, FileInfo exportFileInfo)
    {
        if (data.Count is 0)
        {
            return Error.Validation("List is empty");
        }

        File.WriteAllLines(exportFileInfo.FullName, data);
        return Ok.Success;
    }

    public Result<Error, List<Dictionary<string, string>>> Read(FileReaderOptions fileReaderOptions)
    {
        var lines = File.ReadLines(fileReaderOptions.FileInfo.FullName, Encoding.Latin1)
            .Take(fileReaderOptions.SkipLines..^1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (fileReaderOptions.HasHeaders is false)
        {
            return Error.Custom("Not Implemented");
        }

        var headerColumns = new List<string>();
        var headers = lines.First().Split(fileReaderOptions.Delimiter);
        headerColumns.AddRange(headers);

        return CreateRawDataWithHeaders(fileReaderOptions, lines, headerColumns);
    }

    public Result<Error, IReadOnlyList<string>> Write<T>(IReadOnlyList<T> data)
        where T : notnull =>
        data.Count is 0
            ? Error.Validation("No data provided")
            : FileCreator.WriteObjectsToLines(data, ';').ToList();

    private static List<Dictionary<string, string>> CreateRawDataWithHeaders(FileReaderOptions fileReaderOptions,
        IEnumerable<string> lines, IReadOnlyCollection<string> headerColumns) =>
        lines.Skip(1)
            .Select(ReplaceAmpersand)
            .Select(line => line.Split(fileReaderOptions.Delimiter))
            .Select(values =>
            {
                if (headerColumns.Count != values.Length)
                {
                    throw new ArgumentException(
                        $"Lists not of equal size. Headers = {headerColumns.Count} vs Values = {values.Length}");
                }

                return headerColumns.Zip(values, KeyValuePair.Create)
                    .Where(x => string.IsNullOrWhiteSpace(x.Key) is false);
            })
            .Select(x => x.ToDictionary())
            .ToList();

    private static string ReplaceAmpersand(string s) => s.Replace("&#38;", "&");
}