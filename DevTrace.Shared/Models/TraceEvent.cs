namespace DevTrace.Shared.Models;

/// <summary>
/// Represents a trace event that contains information about an event, such as its source,
/// timestamp, severity level, associated details, and other metadata for logging or debugging purposes.
/// </summary>
public class TraceEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }
    public string? Source { get; set; }
    public string? Level { get; set; }
    public string? CorrelationId { get; set; }
    public string? ClientIp { get; set; }
    public string? Route { get; set; }
    public string? ExceptionDetails { get; set; }
    public string? Payload { get; set; }
    public long DurationMs { get; set; }
    public int StatusCode { get; set; }
    public string HttpMethod { get; set; }
}