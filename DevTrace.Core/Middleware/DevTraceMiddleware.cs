using System.Diagnostics;
using DevTrace.Shared.Models;
using DevTrace.Shared.Stores;
using Microsoft.AspNetCore.Http;

namespace DevTrace.Core.Middleware;

public sealed class DevTraceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var log = new RequestLog
            {
                Timestamp = DateTime.UtcNow,
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
            
            RequestLogStore.Add(log);
        }
    }
}