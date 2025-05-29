using DevTrace.Shared.Models;
using DevTrace.Shared.Options;
using Microsoft.EntityFrameworkCore;

namespace DevTrace.Persistence.EFCore;

public sealed class DevTraceDbContext(DbContextOptions<DevTraceDbContext> options, DevTraceOptions devTraceOptions) : DbContext(options)
{
    public DbSet<TraceEvent> TraceEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        if (!string.IsNullOrWhiteSpace(devTraceOptions.DatabaseSchemaName))
        {
            modelBuilder.HasDefaultSchema(devTraceOptions.DatabaseSchemaName);
        }
        
        modelBuilder.Entity<TraceEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.ExceptionDetails);
            entity.Property(e => e.Source).HasMaxLength(2048);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.ClientIp).HasMaxLength(45);
            entity.Property(e => e.HttpMethod).HasMaxLength(10);

            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.CorrelationId);
            entity.HasIndex(e => e.Source);
            entity.HasIndex(e => e.StatusCode);
        });
    }
}