using Krake.Core.Monads;

namespace Krake.Infrastructure.IO.Common;

public interface IFileReaderService
{
    Result<ErrorBase, List<Dictionary<string, string>>> Read(FileReaderOptions fileReaderOptions);
}