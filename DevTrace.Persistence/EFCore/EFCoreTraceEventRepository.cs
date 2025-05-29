using DevTrace.Core.Repositories;
using DevTrace.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevTrace.Persistence.EFCore;

public sealed class EFCoreTraceEventRepository(DevTraceDbContext dbContext, ILogger<EFCoreTraceEventRepository> logger) : ITraceEventRepository
{
    public async Task AddAsync(TraceEvent traceEvent)
    {
        try
        {
            await dbContext.TraceEvents.AddAsync(traceEvent);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error adding trace event to database.");
            throw;
        }
    }

    public async Task<IEnumerable<TraceEvent>> GetAllAsync(int count = 100, string? level = null)
    {
        try
        {
            IQueryable<TraceEvent> query = dbContext.TraceEvents.OrderByDescending(e => e.Timestamp);
            
            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(te => te.Level == level);
            }
            
            return await query.Take(count).ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving trace events from database.");;
            throw;
        }
    }

    public async Task<TraceEvent?> GetByIdAsync(Guid id)
    {
        try
        {
            return await dbContext.TraceEvents.FindAsync(id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving trace event from database.");
            throw;
        }
    }
}