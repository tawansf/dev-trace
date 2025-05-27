using System.Diagnostics;
using DevTrace.Core.Repositories;
using DevTrace.Shared.Constants;
using DevTrace.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DevTrace.Core.Middleware;

/// <summary>
/// Middleware responsible for tracing HTTP request and response pipeline execution times and handling exceptions.
/// </summary>
/// <remarks>
/// Captures and logs details about HTTP requests in the application, including duration of execution and any exceptions encountered.
/// The collected data is stored using the provided <see cref="ITraceEventRepository"/> implementation.
/// </remarks>
public sealed class DevTraceMiddleware(
    RequestDelegate next,
    ITraceEventRepository traceEventRepository,
    ILogger<DevTraceMiddleware> logger)
{
    /// <summary>
    /// Processes an HTTP request, measures its execution time, captures exceptions if any, and logs these details using a trace event repository.
    /// </summary>
    /// <param name="context">The HttpContext representing the current HTTP request and response.</param>
    /// <returns>A Task representing the asynchronous execution of the middleware.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/devtrace"))
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
            stopwatch.Stop();
            
            var successEvent = CreateTraceEvent(context, stopwatch, null);
            await SafeAddTraceEventAsync(successEvent);
        }
        catch (Exception ex)
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            var errorEvent = CreateTraceEvent(context, stopwatch, ex);
            await SafeAddTraceEventAsync(errorEvent);

            throw;
        }
    }

    /// <summary>
    /// Creates a trace event encapsulating details about a request, such as its source, duration, HTTP method, status code, and exception information if applicable.
    /// </summary>
    /// <param name="context">The HttpContext representing the current HTTP request and response.</param>
    /// <param name="stopwatch">The Stopwatch used to measure the duration of the request.</param>
    /// <param name="exception">An optional Exception object capturing details of an error, if one occurred during the request.</param>
    /// <returns>A TraceEvent containing comprehensive logging information about the processed request.</returns>
    private TraceEvent CreateTraceEvent(HttpContext context, Stopwatch stopwatch, Exception? exception)
    {
        var traceEvent = new TraceEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Source = context.Request.Path.ToString(),
            CorrelationId = context.TraceIdentifier,
            ClientIp = context.Connection.RemoteIpAddress?.ToString(),
            DurationMs = stopwatch.ElapsedMilliseconds,
            HttpMethod = context.Request.Method,
            StatusCode = context.Response.StatusCode
        };

        if (exception != null)
        {
            traceEvent.Level = TraceEventLevels.Error;
            traceEvent.Message = exception.Message;
            traceEvent.ExceptionDetails = exception.ToString();
            if (traceEvent.StatusCode == 0 || (traceEvent.StatusCode == 200 && exception != null))
            {
                traceEvent.StatusCode = 500;
            }
        }
        else
        {
            traceEvent.Level = TraceEventLevels.Info;
            traceEvent.Message = TraceEventMessages.RequestCompletedSuccessfully;
        }

        return traceEvent;
    }

    /// <summary>
    /// Safely attempts to add the provided trace event to the trace event repository, logging any errors that occur during the process.
    /// </summary>
    /// <param name="traceEvent">The trace event containing details about the HTTP request or exception to be stored in the repository.</param>
    /// <returns>A Task representing the completion of the asynchronous operation.</returns>
    private async Task SafeAddTraceEventAsync(TraceEvent traceEvent)
    {
        try
        {
            await traceEventRepository.AddAsync(traceEvent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DevTrace: Failed to add trace event to the repository. TraceId: {TraceId}", traceEvent.Id);
        }
    }
}