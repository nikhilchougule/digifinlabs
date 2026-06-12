using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Common
{
    public abstract class BaseScorecardResult : BaseEntity
    {
        public string ScorecardVersion { get; init; } = string.Empty; //Set once on creation, never change this. This is the version of the scoring logic that was used to compute this scorecard result, used for querying scorecard results by version and for tracking which scoring logic was used for each result.
        public decimal TotalScore { get; set; } //The overall score of the scorecard result, used for sorting scorecard results by score.
        public List<Dictionary<string, object>> ParameterScores { get; set; } = new(); //A list of dictionaries representing the individual results for each scoring category or metric, where the key is the name of the category or metric and the value is the score or result for that category or metric. This allows for flexible and extensible storage of scorecard results without needing to define a fixed schema for each type of scorecard result.>
        public DateTime ComputedAt { get; init; } = DateTime.UtcNow; //immutable timestamp of when the scorecard result was computed, set once on creation and never change this. This allows us to track when each scorecard result was computed and to query scorecard results by date or time range.
    }
}
