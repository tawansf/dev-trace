using DevTrace.Shared.Enums;
using DevTrace.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevTrace.Persistence.EFCore.Hosting;

/// <summary>
/// A hosted service responsible for ensuring that database migrations are applied for the DevTrace application.
/// This service automatically applies migrations at application startup if configured to do so in the options.
/// </summary>
public sealed class DevTraceDatabaseMigratorHostedService(
    IServiceProvider serviceProvider,
    DevTraceOptions devTraceOptions,
    ILogger<DevTraceDatabaseMigratorHostedService> logger): IHostedService
{
    /// <summary>
    /// Starts the hosted service and applies database migrations if configured to do so.
    /// </summary>
    /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of starting the service and applying database migrations.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (devTraceOptions.DatabaseProvider != DatabaseProviderType.None && !string.IsNullOrEmpty(devTraceOptions.ConnectionString) && devTraceOptions.AutoApplyMigrations)
        {
            logger.LogInformation("DevTrace: Verificando e aplicando migrações do banco de dados DevTrace (via IHostedService)...");

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DevTraceDbContext>();
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    logger.LogInformation("DevTrace: Migrações do banco de dados DevTrace aplicadas com sucesso.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DevTrace: Falha crítica ao aplicar migrações do banco de dados DevTrace. A funcionalidade de log pode ser afetada.");
            }
        }
        else
        {
            logger.LogInformation("DevTrace: Aplicação automática de migrações desabilitada ou configuração de banco de dados ausente.");
        }
    }

    /// <summary>
    /// Stops the hosted service and performs any necessary cleanup operations.
    /// </summary>
    /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of stopping the service.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}