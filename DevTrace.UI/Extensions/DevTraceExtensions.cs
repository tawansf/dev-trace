using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DevTrace.UI.Extensions;

public static class DevTraceExtensions
{
    public static IServiceCollection AddDevTrace(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddDevTraceUI();
        return services;
    }

    public static IApplicationBuilder UseDevTrace(this IApplicationBuilder app)
    {
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub("/devtrace/_blazor");
            endpoints.MapFallbackToPage("/DevTrace/_Host", "/devtrace/{*path:nonfile}");
        });

        return app;
    }
}