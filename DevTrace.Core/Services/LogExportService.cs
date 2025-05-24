using System.Text;
using DevTrace.Core.Repositories;

namespace DevTrace.Core.Services;

public class LogExportService(ITraceEventRepository repository) : ILogExportService
{
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
        sb.AppendLine($"Exception: {log.Exception}");

        var content = Encoding.UTF8.GetBytes(sb.ToString());
        return Task.FromResult(content);
    }
}