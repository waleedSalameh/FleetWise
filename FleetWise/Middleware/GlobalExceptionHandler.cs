using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace FleetWise.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        var isConcurrencyConflict = exception is DbUpdateConcurrencyException
            or SqlException { Number: 51000 };
        var statusCode = isConcurrencyConflict
            ? StatusCodes.Status409Conflict
            : StatusCodes.Status500InternalServerError;

        if (isConcurrencyConflict)
        {
            logger.LogWarning(exception, "A concurrent update conflicted with this request.");
        }
        else
        {
            logger.LogError(exception, "Unhandled exception.");
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = isConcurrencyConflict
                ? "The record was changed by another request. Reload it and try again."
                : "An unexpected error occurred.",
            Instance = httpContext.Request.Path
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
