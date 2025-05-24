namespace DevTrace.Shared.Models;

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