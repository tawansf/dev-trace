using System.Diagnostics;
using DevTrace.Core.Repositories;
using DevTrace.Shared.Constants;
using DevTrace.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DevTrace.Core.Middleware;

public sealed class DevTraceMiddleware(RequestDelegate next, ITraceEventRepository traceEventRepository, ILogger<DevTraceMiddleware> logger)
{
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