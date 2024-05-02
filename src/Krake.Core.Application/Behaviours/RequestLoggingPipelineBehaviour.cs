using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using Serilog.Context;

namespace Krake.Core.Application.Behaviours;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IOneOf

{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken token = default)
    {
        var moduleName = GetModuleName(typeof(TRequest).FullName!);
        var requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("Module", moduleName))
        {
            logger.LogInformationProcessRequest(requestName);

            var result = await next().ConfigureAwait(false);

            // KK 2024-05-01
            // This Hacky solution works because this project only uses a custom OneOf Result<TError, TValue>
            // implementation. This circumvents a dependency injection issue, as MediatR's
            // IPipelineBehavior<TRequest, TResponse> only allows for two generics.
            // Here, Index is 1 will be the success value. Index is 0 would be the error value.
            if (result.Index is 1)
            {
                logger.LogInformationCompletedRequest(requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Value, true))
                {
                    logger.LogErrorCompletedRequest(requestName);
                }
            }

            return result;
        }
    }

    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}

internal static partial class RequestLoggingExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Processing request {RequestName}")]
    public static partial void LogInformationProcessRequest(this ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Completed request {RequestName}")]
    public static partial void LogInformationCompletedRequest(this ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Completed request {RequestName} with error")]
    public static partial void LogErrorCompletedRequest(this ILogger logger, string requestName);
}