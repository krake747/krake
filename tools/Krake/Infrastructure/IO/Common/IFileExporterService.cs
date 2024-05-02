using Krake.Core.Monads;

namespace Krake.Infrastructure.IO.Common;

public interface IFileExporterService
{
    Result<ErrorBase, Success> Export(IReadOnlyList<string> data, FileInfo exportFileInfo);
}