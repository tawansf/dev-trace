using System.Diagnostics;
using System.Text.Json;

namespace DevTrace.Core.Middleware;

public sealed class DevTraceMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
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
            var log = new
            {
                context.Request.Path,
                context.Response.StatusCode,
                Duration = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTimeOffset.UtcNow
            };
            
            Console.WriteLine(JsonSerializer.Serialize(log));
        }
        
    }
}