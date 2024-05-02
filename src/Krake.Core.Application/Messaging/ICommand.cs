using Krake.Core.Monads;
using MediatR;

namespace Krake.Core.Application.Messaging;

public interface ICommand<TErrorResponse, TResponse>
    : IRequest<Result<TErrorResponse, TResponse>>, IBaseCommand
    where TErrorResponse : IError;

public interface IBaseCommand;