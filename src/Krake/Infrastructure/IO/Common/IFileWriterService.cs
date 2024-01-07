using Krake.Core;

namespace Krake.Infrastructure.IO.Common;

public interface IFileWriterService
{
    Result<Error, IReadOnlyList<string>> Write<T>(IReadOnlyList<T> data) where T : notnull;
}