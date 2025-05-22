using DevTrace.Shared.Models;

namespace DevTrace.Shared.Stores;

public static class RequestLogStore
{
    private static readonly List<RequestLog> _logs = new();
    private static readonly object _lock = new();

    public static void Add(RequestLog log)
    {
        lock (_lock)
        {
            _logs.Add(log);
            if (_logs.Count > 1000)
                _logs.RemoveAt(0);
        }
    }

    public static IReadOnlyList<RequestLog> GetAll()
    {
        lock (_lock)
        {
            return _logs.ToList();
        }
    }
}