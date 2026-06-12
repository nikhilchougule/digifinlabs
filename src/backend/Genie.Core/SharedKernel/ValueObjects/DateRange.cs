using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable date/time window with a definite start and optional end.
    /// Used for: consent validity, loan tenures, SLA windows, analysis periods.
    /// </summary>
    public sealed record DateRange(DateTime From, DateTime? To = null)
    {
        public bool IsActive => DateTime.UtcNow >= From && (To is null || DateTime.UtcNow <= To);
        public bool HasExpired => To.HasValue && DateTime.UtcNow > To.Value;
        public double? TotalHours => To.HasValue ? (To.Value - From).TotalHours : null;
        public bool Contains(DateTime point) => point >= From && (To is null || point <= To.Value);

        public override string ToString() =>
            To.HasValue ? $"{From:u} → {To:u}" : $"{From:u} → (open)";
    }

}
