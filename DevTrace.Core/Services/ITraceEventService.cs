using DevTrace.Shared.Models;

namespace DevTrace.Core.Services;

public interface ITraceEventService
{
    public Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100);
}