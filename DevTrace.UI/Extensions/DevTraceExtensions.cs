using DevTrace.Core.Middleware;
using DevTrace.Core.Repositories;
using DevTrace.Core.Services;
using DevTrace.Persistence.EFCore;
using DevTrace.Shared.Enums;
using DevTrace.Shared.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace DevTrace.UI.Extensions;

/// <summary>
/// Provides extension methods for configuring and using DevTrace in an ASP.NET Core application.
/// </summary>
public static class DevTraceExtensions
{
    /// <summary>
    /// Adds DevTrace services to the DI container and allows optional configuration of DevTrace options.
    /// </summary>
    /// <param name="services">The IServiceCollection to which DevTrace services will be added.</param>
    /// <param name="configure">An optional Action to configure DevTraceOptions before adding services.</param>
    /// <returns>The IServiceCollection instance with the added DevTrace services.</returns>
    public static IServiceCollection AddDevTrace(this IServiceCollection services, Action<DevTraceOptions>? configure = null)
    {
        var options = new DevTraceOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        
        services.AddLogging();
        
        bool useDatabasePersistence = options.DatabaseProvider != DatabaseProviderType.None && !string.IsNullOrEmpty(options.ConnectionString);

        if (useDatabasePersistence)
        {
            services.AddDbContext<DevTraceDbContext>(dbCtxOptions =>
            {
                switch (options.DatabaseProvider)
                {
                    case DatabaseProviderType.PostgreSQL:
                        dbCtxOptions.UseNpgsql(options.ConnectionString, npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(typeof(DevTraceDbContext).Assembly.FullName);
                        });
                        break;
                    case DatabaseProviderType.SQLServer:
                        dbCtxOptions.UseSqlServer(options.ConnectionString, sqlServerOptions =>
                        {
                            sqlServerOptions.MigrationsAssembly(typeof(DevTraceDbContext).Assembly.FullName);
                        });
                        break;
                    default:
                        var logger = services.BuildServiceProvider().GetService<ILogger<DevTraceOptions>>();
                        logger?.LogWarning("DevTrace: DatabaseProvider '{DbProvider}' não suportado. Usando repositório em memória.", options.DatabaseProvider);
                        useDatabasePersistence = false;
                        break;
                }
            });
        }

        if (useDatabasePersistence)
        {
            services.AddScoped<ITraceEventRepository, EFCoreTraceEventRepository>();
            var logger = services.BuildServiceProvider().GetService<ILogger<DevTraceOptions>>();
            logger?.LogInformation("DevTrace: Usando persistência de banco de dados com o provedor '{DbProvider}'.", options.DatabaseProvider);

            if (options.AutoApplyMigrations)
            {
                try
                {
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DevTraceDbContext>();
                    dbContext.Database.Migrate();
                    logger?.LogInformation("DevTrace: Migrações do banco de dados aplicadas (se houver pendentes).");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "DevTrace: Falha ao aplicar migrações do banco de dados ({DbProvider}).", options.DatabaseProvider);
                }
            }
        }
        else
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<DevTraceOptions>>();
            if (options.DatabaseProvider != DatabaseProviderType.None && string.IsNullOrEmpty(options.ConnectionString))
            {
                logger?.LogWarning("DevTrace: DatabaseProvider '{DbProvider}' foi especificado, mas a ConnectionString está vazia. Usando repositório em memória.", options.DatabaseProvider);
            }
            else
            {
                logger?.LogInformation("DevTrace: Nenhuma configuração de banco de dados válida. Usando repositório em memória.");
            }
            
            services.AddSingleton<ITraceEventRepository, TraceEventRepository>();
        }
        
        services.AddScoped<ITraceEventService, TraceEventService>();
        services.AddScoped<ILogExportService, LogExportService>();

        services.AddDevTraceUI(options);
        
        return services;
    }

    /// <summary>
    /// Adds DevTrace UI services to the DI container based on the provided configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the UI services will be added.</param>
    /// <param name="options">The DevTraceOptions instance containing configuration options for UI services.</param>
    /// <returns>The IServiceCollection instance with the added UI services.</returns>
    public static IServiceCollection AddDevTraceUI(this IServiceCollection services, DevTraceOptions options)
    {
        if (!options.SkipBlazorServerUIServicesRegistration)
        {
            services.AddServerSideBlazor(o =>
            {
                o.DetailedErrors = true;
            });
            
            services.AddRazorPages();
        }
        
        return services;
    }

    /// <summary>
    /// Configures the application to use DevTrace middleware and associated resources, enabling DevTrace functionality within the application's request pipeline.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance used to configure the application's request pipeline.</param>
    /// <returns>The IApplicationBuilder instance with the configured DevTrace middleware and endpoints.</returns>
    public static IApplicationBuilder UseDevTrace(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<DevTraceOptions>();
        var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(DevTraceExtensions).Assembly, options.EmbeddedUiAssetsNamespace),
            RequestPath = options.UiPath
        });

        app.UseMiddleware<DevTraceMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            var blazorHubPath = $"{options.UiPath.TrimEnd('/')}/_blazor";
            var fallbackPagePath = $"{options.UiPath.TrimEnd('/')}/{{*path:nonfile}}";
            endpoints.MapBlazorHub(blazorHubPath);
            endpoints.MapFallbackToPage(options.UiHostPagePath, fallbackPagePath);

            var apiDownloadPath = $"{options.UiPath.TrimEnd('/')}/api/logs/{{id:guid}}/download";

            endpoints.MapGet(apiDownloadPath, async context =>
            {
                if (!string.IsNullOrEmpty(options.DownloadLogsAuthorizationPolicyName))
                {
                    var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();
                    var authorized = await authorizationService.AuthorizeAsync(context.User, options.DownloadLogsAuthorizationPolicyName);
                    if (!authorized.Succeeded)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Forbidden.");
                        return;
                    }
                }
                
                if (!context.Request.RouteValues.TryGetValue("id", out var idObj) || idObj is not string idStr || !Guid.TryParse(idStr, out var id))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("ID do trace inválido ou ausente.");
                    return;
                }
                
                var exportService = context.RequestServices.GetRequiredService<ILogExportService>();
                var apiLogger = loggerFactory.CreateLogger("DevTrace.ApiEndpoints");

                try
                {
                    byte[] contentBytes = await exportService.ExportAsTextAsync(id);

                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"trace_{id}.txt\"");
                    context.Response.Headers.Append("Content-Length", contentBytes.Length.ToString());

                    await context.Response.Body.WriteAsync(contentBytes);
                }
                catch (FileNotFoundException fnfEx)
                {
                    apiLogger.LogWarning(fnfEx, "Log não encontrado para download. TraceId: {TraceId}", id);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Log não encontrado.");
                }
                catch (Exception ex)
                {
                    apiLogger.LogError(ex, "Erro ao gerar arquivo de log para download. TraceId: {TraceId}", id);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"Erro interno ao gerar arquivo de log.");
                }
            });
        });
        
        return app;
    }
}