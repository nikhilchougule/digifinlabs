namespace SharedKernel.Interfaces
{
    /// <summary>
    /// Marks an entity as append-only: rows may be inserted but never updated or deleted.
    /// Used for regulatory audit trails (stage logs, approvals, outbox).
    /// The domain layer signals intent here; the persistence layer enforces it
    /// (EF Core interceptor + a DB trigger that rejects UPDATE/DELETE).
    /// </summary>
    public interface IAppendOnly { }
}
