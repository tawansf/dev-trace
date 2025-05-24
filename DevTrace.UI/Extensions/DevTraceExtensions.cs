using DevTrace.Core.Middleware;
using DevTrace.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        app.UseMiddleware<DevTraceMiddleware>();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub("/devtrace/_blazor");
            endpoints.MapFallbackToPage("/DevTrace/_Host", "/devtrace/{*path:nonfile}");

            endpoints.MapGet("/devtrace/api/logs/{id:guid}/download", async context =>
            {
                if (!context.Request.RouteValues.TryGetValue("id", out var idObj) || idObj is not string idStr || !Guid.TryParse(idStr, out var id))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid or missing trace ID.");
                    return;
                }

                var service = context.RequestServices.GetRequiredService<ILogExportService>();

                try
                {
                    var content = await service.ExportAsTextAsync(id);

                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.Headers.ContentDisposition = $"attachment; filename=\"trace_{id}.txt\"";
                    context.Response.Headers.ContentLength = content.Length;

                    await context.Response.Body.WriteAsync(content, 0, content.Length);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"Error generating log file: {ex.Message}");
                }
            });

        });

        return app;
    }
}