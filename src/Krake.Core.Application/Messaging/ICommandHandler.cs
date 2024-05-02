using Krake.Core.Monads;
using MediatR;

namespace Krake.Core.Application.Messaging;

public interface ICommandHandler<in TCommand, TErrorResponse, TResponse>
    : IRequestHandler<TCommand, Result<TErrorResponse, TResponse>>
    where TCommand : ICommand<TErrorResponse, TResponse>
    where TErrorResponse : IError;