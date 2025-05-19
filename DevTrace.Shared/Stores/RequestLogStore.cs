using DevTrace.Shared.Models;

namespace DevTrace.Shared.Stores;

public static class RequestLogStore
{
    private static readonly List<RequestLog> _logs = new();

    public static IReadOnlyList<RequestLog> Logs => _logs.AsReadOnly();

    public static void Add(RequestLog log)
    {
        _logs.Add(log);
        if (_logs.Count > 1000)
        {
            _logs.RemoveAt(0);
        }
    }
    
    public static void Clear() => _logs.Clear();
}