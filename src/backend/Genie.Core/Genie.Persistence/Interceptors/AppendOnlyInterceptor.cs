using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Interfaces;

namespace Genie.Persistence.Interceptors
{
    /// <summary>
    /// Blocks any UPDATE or DELETE on entities that implement IAppendOnly.
    /// Enforces immutability of audit trails (ConsentLog, AuditLog, ApplicationStageLog)
    /// at the domain layer — the DB trigger is the second line of defence.
    /// </summary>
    public sealed class AppendOnlyInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            Enforce(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            Enforce(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void Enforce(DbContext? ctx)
        {
            if (ctx is null) return;

            var violations = ctx.ChangeTracker.Entries()
                .Where(e => e.Entity is IAppendOnly &&
                            e.State is EntityState.Modified or EntityState.Deleted)
                .Select(e => e.Entity.GetType().Name)
                .ToList();

            if (violations.Count > 0)
                throw new InvalidOperationException(
                    $"Attempted to modify or delete append-only entity(ies): " +
                    $"{string.Join(", ", violations)}. " +
                    "These rows are immutable by design to protect the regulatory audit trail.");
        }
    }
}
