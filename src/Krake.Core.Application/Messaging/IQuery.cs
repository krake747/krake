using Krake.Core.Monads;
using MediatR;

namespace Krake.Core.Application.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>;

public interface IQuery<TErrorResponse, TResponse> : IRequest<Result<TErrorResponse, TResponse>>
    where TErrorResponse : IError;