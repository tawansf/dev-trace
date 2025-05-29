using System.Collections.Concurrent;
using DevTrace.Shared.Models;

namespace DevTrace.Core.Repositories;

/// <summary>
/// A repository for managing trace events, commonly used for logging and debugging purposes.
/// </summary>
/// <remarks>
/// Provides methods to asynchronously add and retrieve trace events. Trace events are stored
/// in a thread-safe manner, using a <see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/>
/// to maintain the events in memory.
/// </remarks>
public class TraceEventRepository : ITraceEventRepository
{
    private readonly ConcurrentQueue<TraceEvent> _events = new();

    /// <summary>
    /// Asynchronously adds a trace event to the internal storage.
    /// </summary>
    /// <param name="traceEvent">The trace event to be added to the repository.</param>
    /// <returns>A task that represents the completion of the add operation.</returns>
    public Task AddAsync(TraceEvent traceEvent)
    {
        _events.Enqueue(traceEvent);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously retrieves all trace events stored in the repository.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing a collection of all trace events.</returns>
    public Task<IEnumerable<TraceEvent>> GetAllAsync(int count = 100, string? level = null)
    {
        return Task.FromResult<IEnumerable<TraceEvent>>(_events.ToArray());
    }

    public Task<TraceEvent?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TraceEvent>> GetRecentAsync(int maxCount = 100, string? level = null)
    {
        throw new NotImplementedException();
    }
}