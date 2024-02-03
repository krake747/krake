using Krake.Core;

namespace Krake.Infrastructure.IO.Common;

public interface IFileReaderService
{
    Result<Error, List<Dictionary<string, string>>> Read(FileReaderOptions fileReaderOptions);
}