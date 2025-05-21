namespace DevTrace.Core.Entities;

public class TraceEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }
    public string? Source { get; set; }
    public string? Level { get; set; }
}