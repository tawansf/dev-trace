using DevTrace.Core.Entities;
using DevTrace.Shared.Stores;

namespace DevTrace.Core.Services;

public class TraceEventService: ITraceEventService
{
    public Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100)
    {
        var logs = RequestLogStore.GetAll();

        var events = logs.Select(log => new TraceEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = log.Timestamp,
            Message = $"{log.Method} {log.Path}",
            Source = "Middleware",
            Level = log.StatusCode >= 500 ? "Error" : "Info"
        }).ToList();

        return Task.FromResult((IReadOnlyList<TraceEvent>)events);
    }
}