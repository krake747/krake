using Krake.Core;
using Krake.Infrastructure.IO.Models;

namespace Krake.Infrastructure.IO;

public interface IFileReaderService
{
    Result<Error, List<Dictionary<string, string>>> Read(FileReaderOptions fileReaderOptions);
}