using DevTrace.Core.Entities;

namespace DevTrace.Core.Services;

public interface ITraceEventService
{
    Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100);
}