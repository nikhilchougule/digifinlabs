using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Underwriting.Entities
{
    /// <summary>
    /// Bank-statement analysis for an Application — derives from BaseAnalysis (the kernel analysis base).
    ///
    /// DESIGN RULES:
    ///   - IMMUTABLE results: a re-run is a new row; existing rows are never updated.
    ///   - RequestId is the idempotency key for the analysis request.
    ///   - ComputeScore() produces the 0–100 banking score from the captured metrics.
    /// </summary>
    public sealed class BankingAnalysis : BaseAnalysis
    {
        public required string RequestId { get; init; }

        public Money? AvgMonthlyCredits { get; set; }
        public Money? AvgMonthlyDebits { get; set; }
        public Money? AvgEodBalance { get; set; }
        public Money? MinEodBalance { get; set; }

        public Dictionary<string, object> EmiObligationsDetected { get; set; } = new();

        public int BounceCount12m { get; set; }
        public int InwardChequeReturnCount { get; set; }
        public int OutwardChequeReturnCount { get; set; }

        public decimal CashDepositPercentage { get; set; }   // AML signal if unusually high
        public decimal SalaryCreditRegularity { get; set; }  // 0-100
        public decimal EmiTrackRecord { get; set; }          // 0-100

        public decimal BankingScore { get; set; }            // 0-100, = ComputeScore()

        /// <summary>
        /// 0–100 banking score: rewards salary regularity and EMI track record,
        /// penalises bounces. Illustrative weighting — real model owned by Risk &amp; Analytics.
        /// </summary>
        protected override decimal ComputeScore()
        {
            var bouncePenalty = Math.Min(BounceCount12m * 5m, 40m);
            var raw = (SalaryCreditRegularity * 0.5m) + (EmiTrackRecord * 0.5m) - bouncePenalty;
            return Math.Clamp(raw, 0m, 100m);
        }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (SubjectId == Guid.Empty)
                yield return "SubjectId (ApplicationId) must not be empty.";

            if (string.IsNullOrWhiteSpace(RequestId))
                yield return "RequestId (idempotency key) must not be empty.";

            if (AnalysisPeriodMonths <= 0)
                yield return "AnalysisPeriodMonths must be greater than zero.";

            if (BankingScore is < 0m or > 100m)
                yield return "BankingScore must be between 0 and 100.";
        }

        public override string GetDisplayName() => $"BankingAnalysis[{BankingScore:N0}]:{SubjectId}";
    }
}
