using System.Text.RegularExpressions;
using Krake.Cli.Features.Common;
using Krake.Core;
using Krake.Infrastructure.IO.Common;
using Krake.Infrastructure.IO.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features.FinanceData;

public sealed class FinanceDataFileManager([FromKeyedServices("finance-data")] DirectoryManager directoryManager)
    : IFileReaderService
{
    public DirectoryManager DirectoryManager { get; } = directoryManager;

    public Result<Error, List<Dictionary<string, string>>> Read(FileReaderOptions fileReaderOptions)
    {
        var lines = File.ReadLines(fileReaderOptions.FileInfo.FullName, fileReaderOptions.Encoding)
            .Take(fileReaderOptions.SkipLines..^1)
            .Where(line => string.IsNullOrWhiteSpace(line) is false)
            .ToList();

        if (fileReaderOptions.HasHeaders is false)
        {
            return Error.Custom("Not Implemented");
        }

        var headerColumns = new List<string>();
        var headers = lines[0].Split(fileReaderOptions.Delimiter);
        headerColumns.AddRange(headers);

        var realCommaDelimiterIndices = lines.Take(1..10).Select(line =>
        {
            var indices = Regex.Matches(line, "(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
            var values =
                (from Match m in indices
                    select m.Value.Contains(',')
                        ? m.Value.Replace(",", "").Replace("\"", "")
                        : m.Value).ToArray();

            return values;
        }).ToArray();


        return new List<Dictionary<string, string>>();
    }


    // private static int[] FindTrueComma(string s) =>
    //     ;
}