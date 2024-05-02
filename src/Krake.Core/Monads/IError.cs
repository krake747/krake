namespace Krake.Core.Monads;

public interface IError
{
    string Code { get; }
    string Message { get; }
    ErrorType Type { get; }
}