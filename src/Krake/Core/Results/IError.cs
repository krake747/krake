namespace Krake.Core.Results;

public interface IError
{
    string Code { get; }
    string Message { get; }
    ErrorType Type { get; }
}