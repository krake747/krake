using Krake.Core;

namespace Krake.Infrastructure.IO;

public interface IFileWriterService
{
    Result<Error, IReadOnlyList<string>> Write<T>(IReadOnlyList<T> data) where T : notnull;
}