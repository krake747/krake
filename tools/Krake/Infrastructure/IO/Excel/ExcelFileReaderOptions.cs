namespace Krake.Infrastructure.IO.Excel;

public sealed class ExcelFileReaderOptions
{
    public bool HasHeaders { get; set; } = true;
    public bool SkipEmptyRows { get; set; } = true;
}