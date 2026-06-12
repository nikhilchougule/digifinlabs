using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Common
{
    public abstract class BaseAnalysis : BaseAuditableEntity
    {
        /// <summary>
        /// Polymorphic FK — the entity this analysis is about.
        /// e.g. PersonalProfile.Id, BusinessProfile.Id, or Application.Id.
        /// </summary>
        public Guid SubjectId { get; init; }
        /// <summary>Discriminator for SubjectId. e.g. "PersonalProfile", "BusinessProfile", "Application".</summary>
        public string SubjectType { get; init; } = string.Empty;
        public int AnalysisPeriodMonths { get; set; } //The number of months of data that this analysis covers, used for querying analyses by period.
        public decimal Score { get;  set; } //The overall score of the analysis, used for sorting analyses by score.
        public DateTime AnalyzedAt { get; set; }
        public Dictionary<string, object> Anomalies { get; set; } = new();

        protected abstract decimal ComputeScore(); //Derived classes must implement this method to compute the score based on their specific analysis logic.
        protected virtual void PostProcessing() //Derived classes can override this method to perform any additional processing after the score is computed, such as populating the Anomalies dictionary with any detected anomalies.
        {
            //calls ComputeScore() to calculate the score for this analysis, and then performs any additional post-processing logic.
            //Base post-processing logic can go here, such as logging the analysis results, sending notifications, etc.
        }

    }
}
