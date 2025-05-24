using DevTrace.Core.Repositories;
using DevTrace.Shared.Models;

namespace DevTrace.Core.Services;

public class TraceEventService(ITraceEventRepository repository) : ITraceEventService
{
    public async Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100)
    {
        var allEvents = await repository.GetAllAsync();
        return allEvents.TakeLast(maxCount).ToList().AsReadOnly();
    }
}