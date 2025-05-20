using DevTrace.Core.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DevTrace.Core.Extensions;

public static class DevTraceMiddlewareExtensions
{
    public static IApplicationBuilder UseDevTrace(this IApplicationBuilder app)
    {
        app.UseMiddleware<DevTraceMiddleware>();
        return app;
    }
}