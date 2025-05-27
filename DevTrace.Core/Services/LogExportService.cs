using System.Text;
using DevTrace.Core.Repositories;

namespace DevTrace.Core.Services;

/// <summary>
/// Provides functionality for exporting logs in text format.
/// </summary>
/// <remarks>
/// This service retrieves a log entry by its unique identifier and exports its details as a text file.
/// </remarks>
public class LogExportService(ITraceEventRepository repository) : ILogExportService
{
    /// <summary>
    /// Exports a specific trace event log as a text file in byte array format.
    /// </summary>
    /// <param name="id">The unique identifier of the log to be exported.</param>
    /// <returns>A task representing the asynchronous operation, containing a byte array of the exported log content in text format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the log with the specified ID is not found in the repository.</exception>
    public Task<byte[]> ExportAsTextAsync(Guid id)
    {
        var log = repository.GetAllAsync().Result.FirstOrDefault(x => x.Id == id);
        if (log == null)
            throw new InvalidOperationException("Log não encontrado.");

        var sb = new StringBuilder();
        sb.AppendLine($"Timestamp: {log.Timestamp}");
        sb.AppendLine($"Level: {log.Level}");
        sb.AppendLine($"Source: {log.Source}");
        sb.AppendLine($"Message: {log.Message}");
        sb.AppendLine($"Client IP: {log.ClientIp}");
        sb.AppendLine($"Duration (ms): {log.DurationMs}");
        sb.AppendLine($"Correlation ID: {log.CorrelationId}");
        sb.AppendLine($"Exception: {log.ExceptionDetails}");

        var content = Encoding.UTF8.GetBytes(sb.ToString());
        return Task.FromResult(content);
    }
}