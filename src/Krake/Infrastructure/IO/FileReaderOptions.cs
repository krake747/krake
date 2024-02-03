using System.Text;

namespace Krake.Infrastructure.IO;

public sealed class FileReaderOptions
{
    public required FileInfo FileInfo { get; init; }
    public char Delimiter { get; set; }
    public bool HasHeaders { get; set; } = true;
    public int SkipLines { get; set; }
    public Encoding Encoding { get; set; } = Encoding.UTF8;
}