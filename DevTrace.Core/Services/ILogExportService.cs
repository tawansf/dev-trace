namespace DevTrace.Core.Services;

/// <summary>
/// Defines a service for exporting log events.
/// </summary>
public interface ILogExportService
{
    /// <summary>
    /// Asynchronously exports a log event identified by the given event ID as a text file.
    /// </summary>
    /// <param name="eventId">The unique identifier of the log event to export.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the text content of the exported log event.</returns>
    Task<byte[]> ExportAsTextAsync(Guid eventId);
}