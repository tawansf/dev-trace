using DevTrace.Core.Repositories;
using DevTrace.Shared.Models;

namespace DevTrace.Core.Services;

/// <summary>
/// Provides a service implementation for managing and retrieving trace events. This service
/// interacts with a repository to fetch trace events and provides features such as retrieving
/// a limited number of the most recent trace events.
/// </summary>
public class TraceEventService(ITraceEventRepository repository) : ITraceEventService
{
    /// <summary>
    /// Retrieves a specified number of the most recent trace events asynchronously.
    /// </summary>
    /// <param name="maxCount">The maximum number of trace events to retrieve. Defaults to 100.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of trace events.</returns>
    public async Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100)
    {
        var allEvents = await repository.GetAllAsync();
        return allEvents.TakeLast(maxCount).ToList().AsReadOnly();
    }
}