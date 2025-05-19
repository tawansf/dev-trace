namespace DevTrace.Shared.Models;

public class RequestLog
{
    public DateTime Timestamp { get; set; }
    public string Method { get; set; } = null!;
    public string Path { get; set; } = null!;
    public int StatusCode { get; set; }
    public long ElapsedMilliseconds { get; set; }
}