using DevTrace.Core.Middleware;

namespace DevTrace.Core.Extensions;

public static class DevTraceMiddleareExtensions
{
    public static IServiceCollection AddDevTraceDashboard(this IServiceCollection services)
    {
        services.AddRazorPages();
        return services;
    }
    public static IApplicationBuilder UseDevTrace(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DevTraceMiddleware>();
    }
    
    public static IApplicationBuilder UseDevTraceDashboard(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            e.MapRazorPages();
        });

        return app;
    }
}