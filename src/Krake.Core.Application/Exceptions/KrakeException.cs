using static Krake.Core.Monads.ErrorBase;

namespace Krake.Core.Application.Exceptions;

internal sealed class KrakeException(string requestName, Error? error = default, Exception? innerException = default)
    : Exception("Application exception", innerException)
{
    public string RequestName { get; } = requestName;
    public Error? Error { get; } = error;
}