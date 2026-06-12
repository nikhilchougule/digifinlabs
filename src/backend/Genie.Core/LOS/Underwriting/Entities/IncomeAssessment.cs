using Los.Underwriting.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Underwriting.Entities
{
    /// <summary>
    /// Income assessment for an Application — salaried, business, rental, or mixed.
    ///
    /// DESIGN RULES:
    ///   - All monetary fields use Money.
    ///   - IncomeVarianceFlag is raised when GST vs ITR vs Banking-derived income disagree beyond threshold.
    ///   - AdjustedIncome is the underwriter's final judgement after triangulation.
    /// </summary>
    public sealed class IncomeAssessment : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required AssessmentType AssessmentType { get; init; }

        // Salaried
        public Money? GrossMonthlySalary { get; set; }
        public Money? NetTakeHome { get; set; }
        public decimal? EmployerStabilityScore { get; set; }   // 0-100
        public int? VintageWithEmployerMonths { get; set; }

        // Business
        public Money? AvgMonthlyTurnover { get; set; }
        public Money? GstDeclaredTurnover { get; set; }
        public Money? ItrDeclaredIncome { get; set; }
        public Money? BankingDerivedIncome { get; set; }
        public bool IncomeVarianceFlag { get; set; }
        public Money? AdjustedIncome { get; set; }

        public Dictionary<string, object> IncomeSourceBreakdown { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (EmployerStabilityScore is < 0m or > 100m)
                yield return "EmployerStabilityScore must be between 0 and 100.";
        }

        public override string GetDisplayName() => $"IncomeAssessment[{AssessmentType}]:{ApplicationId}";
    }
}
