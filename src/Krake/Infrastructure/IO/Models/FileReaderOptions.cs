namespace Krake.Infrastructure.IO.Models;

public sealed class FileReaderOptions
{
    public required FileInfo FileInfo { get; init; }
    public char Delimiter { get; set; }
    public bool HasHeaders { get; set; } = true;
    public int SkipLines { get; set; }
}