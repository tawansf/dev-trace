using DevTrace.Core.Middleware;
using DevTrace.Shared.Stores;

namespace DevTrace.UI.Extensions;

public static class DevTraceMiddlewareExtensions
{
    public static IApplicationBuilder UseDevTraceDashboard(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapGet("/devtrace/logs", context =>
            {
                var logs = RequestLogStore.Logs;
                return context.Response.WriteAsJsonAsync(logs);
            });
        });
        return app;
    }
}