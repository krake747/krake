using Krake.Core.Monads;
using MediatR;

namespace Krake.Core.Application.Messaging;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;

public interface IQueryHandler<in TQuery, TErrorResponse, TResponse>
    : IRequestHandler<TQuery, Result<TErrorResponse, TResponse>>
    where TQuery : IQuery<TErrorResponse, TResponse>
    where TErrorResponse : IError;