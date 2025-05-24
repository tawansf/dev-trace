using System.Collections.Concurrent;
using DevTrace.Shared.Models;

namespace DevTrace.Core.Repositories;

public class TraceEventRepository : ITraceEventRepository
{
    private readonly ConcurrentQueue<TraceEvent> _events = new();

    public Task AddAsync(TraceEvent traceEvent)
    {
        _events.Enqueue(traceEvent);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<TraceEvent>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<TraceEvent>>(_events.ToArray());
    }
}