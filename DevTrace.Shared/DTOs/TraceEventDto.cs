namespace DevTrace.Shared.DTOs;

public class TraceEventDto
{
    public string Message { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}