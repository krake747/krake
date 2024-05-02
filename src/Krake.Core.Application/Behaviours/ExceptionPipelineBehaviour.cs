using Krake.Core.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Krake.Core.Application.Behaviours;

internal sealed class ExceptionHandlingPipelineBehaviour<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken token = default)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogErrorUnhandledException(exception, typeof(TRequest).Name);

            throw new KrakeException(typeof(TRequest).Name, innerException: exception);
        }
    }
}

internal static partial class ExceptionHandlingLoggingExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception for {RequestName}")]
    public static partial void LogErrorUnhandledException(this ILogger logger, Exception exception, string requestName);
}