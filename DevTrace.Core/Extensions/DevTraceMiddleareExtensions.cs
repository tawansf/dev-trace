using DevTrace.Core.Middleware;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevTrace.Core.Extensions;

public static class DevTraceMiddleareExtensions
{
    public static IServiceCollection AddDevTraceDashboard(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AddPageRoute("/Dashboard", "/devtrace");
        });
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