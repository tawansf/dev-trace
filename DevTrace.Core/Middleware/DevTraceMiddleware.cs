using System.Diagnostics;
using DevTrace.Core.Repositories;
using DevTrace.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace DevTrace.Core.Middleware;

public sealed class DevTraceMiddleware(RequestDelegate next, ITraceEventRepository traceEventRepository)
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
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var errorEvent = new TraceEvent
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Message = ex.Message,
                Exception = ex.ToString(),
                Source = context.Request.Path,
                CorrelationId = context.TraceIdentifier,
                ClientIp = context.Connection.RemoteIpAddress?.ToString(),
                DurationMs = stopwatch.ElapsedMilliseconds
            };

            await traceEventRepository.AddAsync(errorEvent);

            throw;
        }

        stopwatch.Stop();

        var traceEvent = new TraceEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Level = "Info",
            Message = "Request completed",
            Source = context.Request.Path,
            CorrelationId = context.TraceIdentifier,
            ClientIp = context.Connection.RemoteIpAddress?.ToString(),
            DurationMs = stopwatch.ElapsedMilliseconds
        };

        await traceEventRepository.AddAsync(traceEvent);
    }
}