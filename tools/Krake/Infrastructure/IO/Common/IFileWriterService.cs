using Krake.Core.Monads;

namespace Krake.Infrastructure.IO.Common;

public interface IFileWriterService
{
    Result<ErrorBase, IReadOnlyList<string>> Write<T>(IReadOnlyList<T> data) where T : notnull;
}