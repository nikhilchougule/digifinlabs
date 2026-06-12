using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Common;

namespace Genie.Persistence.Interceptors
{
    /// <summary>
    /// Automatically stamps UpdatedAt = UtcNow on every Modified BaseEntity
    /// so application code never has to set it manually.
    /// </summary>
    public sealed class AuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            StampTimestamps(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            StampTimestamps(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void StampTimestamps(DbContext? ctx)
        {
            if (ctx is null) return;

            foreach (var entry in ctx.ChangeTracker.Entries<BaseEntity>()
                         .Where(e => e.State == EntityState.Modified))
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
