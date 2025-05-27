using DevTrace.Shared.Models;

namespace DevTrace.Core.Repositories;

/// <summary>
/// Defines an interface for a repository that manages trace events.
/// </summary>
/// <remarks>
/// Provides methods for adding and retrieving trace events, intended for use in logging and debugging scenarios.
/// </remarks>
public interface ITraceEventRepository
{
    /// <summary>
    /// Adds a new trace event to the repository.
    /// </summary>
    /// <param name="traceEvent">The trace event to be added, containing details such as timestamp, message, level, and associated metadata.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(TraceEvent traceEvent);

    /// <summary>
    /// Retrieves all trace events from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all trace events.</returns>
    Task<IEnumerable<TraceEvent>> GetAllAsync();
}