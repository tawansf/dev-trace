using DevTrace.UI.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DevTrace.UI.Extensions
{
    public static class DevTraceUIExtensions
    {
        public static IServiceCollection AddDevTraceUI(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            return services;
        }

        public static IApplicationBuilder UseDevTraceUI(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/devtrace/{*catchall}", "/DevTrace/_Host");
            });
            return app;
        }
    }
}