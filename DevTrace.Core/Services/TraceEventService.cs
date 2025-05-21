using DevTrace.Core.Entities;

namespace DevTrace.Core.Services;

public class TraceEventService: ITraceEventService
{
    public Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100)
    {
        var list = new List<TraceEvent>
        {
            new() { Id = Guid.NewGuid(), Timestamp = DateTime.Now, Level = "Info", Message = "Evento simulado 1" },
            new() { Id = Guid.NewGuid(), Timestamp = DateTime.Now, Level = "Warning", Message = "Evento simulado 2" }
        };
        return Task.FromResult((IReadOnlyList<TraceEvent>)list);
    }
}