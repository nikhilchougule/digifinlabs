using SharedKernel.Enums;

namespace SharedKernel.ValueObjects
{
    /// <summary>
    /// A single typed predicate in the rule DSL — the compile-checked counterpart of the
    /// raw JSONB { field, operator, value } shape used across services.
    /// Evaluated by the risk rule engine and by lender eligibility matching.
    ///
    /// Example: new RuleCondition("bounce_count_12m", RuleOperator.Gt, 3)
    ///   → "reject/flag when bounces in the last 12 months exceed 3".
    /// </summary>
    public sealed record RuleCondition(string Field, RuleOperator Operator, object? Value)
    {
        public IEnumerable<string> Validate()
        {
            if (string.IsNullOrWhiteSpace(Field))
                yield return "RuleCondition.Field must not be empty.";

            // Exists / NotExists are unary — every other operator needs a comparison value.
            var unary = Operator is RuleOperator.Exists or RuleOperator.NotExists;
            if (!unary && Value is null)
                yield return $"RuleCondition with operator {Operator} requires a Value.";
        }
    }
}
