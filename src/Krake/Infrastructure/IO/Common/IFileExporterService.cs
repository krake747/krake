using Krake.Core;

namespace Krake.Infrastructure.IO.Common;

public interface IFileExporterService
{
    Result<Error, Success> Export(IReadOnlyList<string> data, FileInfo exportFileInfo);
}