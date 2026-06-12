namespace SharedKernel.Enums
{
    /// <summary>
    /// Comparison operators for the JSONB rule DSL used by risk rules and lender eligibility criteria.
    /// A rule reads { field, operator, value } and evaluates the operator against a data point.
    /// </summary>
    public enum RuleOperator
    {
        Gt,
        Lt,
        Gte,
        Lte,
        Eq,
        Neq,
        In,
        NotIn,
        Exists,
        NotExists
    }
}
