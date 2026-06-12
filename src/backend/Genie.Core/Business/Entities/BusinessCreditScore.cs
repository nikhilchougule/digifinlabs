using Business.ValueObjects;
using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// The proprietary MSME credit score (0–1000) — DigifinLabs' core moat.
    /// Derives from BaseScorecardResult, reusing ScorecardVersion + ComputedAt.
    ///
    /// DESIGN RULES:
    ///   - APPEND-ONLY: a recompute inserts a NEW row. Old scores are never updated or deleted —
    ///     model retraining must not retroactively alter past decisions. Query latest by ComputedAt.
    ///   - ScorecardVersion (from the base) is mandatory — tracks which model produced the score.
    ///   - Components is the fixed-shape weighted breakdown; Explainability is free-form
    ///     (SHAP values / plain-English reasons) for RBI Fair Practice Code transparency.
    /// </summary>
    public sealed class BusinessCreditScore : BaseScorecardResult
    {
        public required Guid BusinessId { get; init; }

        /// <summary>Final score on a 0–1000 scale.</summary>
        public required decimal Score { get; init; }

        public required RiskBand RiskBand { get; init; }

        /// <summary>Typed weighted breakdown (GST/Banking/Bureau/Vintage/Social).</summary>
        public required ScoreComponents Components { get; init; }

        /// <summary>SHAP values / plain-English reasons (JSONB). Regenerated each compute.</summary>
        public Dictionary<string, object> Explainability { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (BusinessId == Guid.Empty)
                yield return "BusinessId must not be empty.";

            if (string.IsNullOrWhiteSpace(ScorecardVersion))
                yield return "ScorecardVersion is mandatory for AI/model governance.";

            if (Score < 0m || Score > 1000m)
                yield return "Score must be between 0 and 1000.";

            foreach (var err in Components.Validate())
                yield return err;
        }

        public override string GetDisplayName() => $"BizScore[{Score:N0}|{RiskBand}]:{Id}";
    }
}
