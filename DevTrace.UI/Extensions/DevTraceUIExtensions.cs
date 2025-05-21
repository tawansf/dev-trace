using DevTrace.Core.Services;
using DevTrace.UI.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DevTrace.UI.Extensions
{
    public static class DevTraceUIExtensions
    {
        public static IServiceCollection AddDevTraceUI(this IServiceCollection services)
        {
            services.AddScoped<ITraceEventService, TraceEventService>();
            
            return services;
        }
    }
}