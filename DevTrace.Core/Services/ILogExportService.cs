namespace DevTrace.Core.Services;

public interface ILogExportService
{
    Task<byte[]> ExportAsTextAsync(Guid eventId);
}