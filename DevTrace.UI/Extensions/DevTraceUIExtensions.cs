using DevTrace.Core.Repositories;
using DevTrace.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevTrace.UI.Extensions
{
    public static class DevTraceUIExtensions
    {
        public static IServiceCollection AddDevTraceUI(this IServiceCollection services)
        {
            services.AddSingleton<ITraceEventRepository, TraceEventRepository>();
            
            services.AddScoped<ILogExportService, LogExportService>();
            services.AddScoped<ITraceEventService, TraceEventService>();
            
            return services;
        }
    }
}