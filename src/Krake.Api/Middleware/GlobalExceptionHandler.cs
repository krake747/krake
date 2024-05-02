using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Krake.Api.Middleware;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken token = default)
    {
        logger.LogErrorUnhandledException(exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, token);

        return true;
    }
}

internal static partial class GlobalExceptionHandlerLoggingExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception occurred")]
    public static partial void LogErrorUnhandledException(this ILogger logger, Exception exception);
}