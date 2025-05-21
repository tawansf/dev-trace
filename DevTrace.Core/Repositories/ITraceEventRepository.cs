using DevTrace.Core.Entities;

namespace DevTrace.Core.Repositories;

public interface ITraceEventRepository
{
    Task AddAsync(TraceEvent traceEvent);
    Task<IEnumerable<TraceEvent>> GetAllAsync();
}