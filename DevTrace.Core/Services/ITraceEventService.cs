using DevTrace.Shared.Models;

namespace DevTrace.Core.Services;

/// <summary>
/// Defines a service for managing and retrieving trace events, typically used for logging, debugging,
/// and monitoring purposes in an application. This service provides functionality to fetch a collection
/// of trace events according to specified criteria.
/// </summary>
public interface ITraceEventService
{
    /// <summary>
    /// Retrieves a collection of trace events, limited to a maximum specified count, in ascending order
    /// of their arrival or creation.
    /// </summary>
    /// <param name="maxCount">The maximum number of trace events to retrieve. Defaults to 100 if not specified.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of trace events.</returns>
    public Task<IReadOnlyList<TraceEvent>> GetEventsAsync(int maxCount = 100);
}