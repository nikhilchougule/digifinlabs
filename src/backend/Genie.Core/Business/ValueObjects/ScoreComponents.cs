namespace Business.ValueObjects
{
    /// <summary>
    /// Typed, weighted breakdown behind the proprietary BusinessCreditScore.
    /// Weights are fixed by the scoring model (GST 25 / Banking 30 / Bureau 20 / Vintage 15 / Social 10)
    /// and MUST sum to 100. Each sub-score is on a 0–100 scale before weighting.
    ///
    /// This is the fixed-shape counterpart to the free-form Explainability JSONB — persisted as a
    /// structured column/owned type so it can be queried, not just displayed.
    /// </summary>
    public sealed record ScoreComponents
    {
        public decimal GstScore { get; init; }      // weight 25%
        public decimal BankingScore { get; init; }  // weight 30%
        public decimal BureauScore { get; init; }   // weight 20%
        public decimal VintageScore { get; init; }  // weight 15%
        public decimal SocialScore { get; init; }   // weight 10%

        public const decimal GstWeight = 25m;
        public const decimal BankingWeight = 30m;
        public const decimal BureauWeight = 20m;
        public const decimal VintageWeight = 15m;
        public const decimal SocialWeight = 10m;

        /// <summary>Model invariant: the five component weights always sum to 100.</summary>
        public static bool WeightsSumTo100 =>
            GstWeight + BankingWeight + BureauWeight + VintageWeight + SocialWeight == 100m;

        /// <summary>Weighted total on a 0–100 scale; multiply by 10 for the 0–1000 BusinessCreditScore.</summary>
        public decimal WeightedTotal() =>
            (GstScore * GstWeight
             + BankingScore * BankingWeight
             + BureauScore * BureauWeight
             + VintageScore * VintageWeight
             + SocialScore * SocialWeight) / 100m;

        public IEnumerable<string> Validate()
        {
            // Weights are fixed model constants and always sum to 100 (asserted by WeightsSumTo100).
            foreach (var (name, value) in new[]
                     {
                         (nameof(GstScore), GstScore),
                         (nameof(BankingScore), BankingScore),
                         (nameof(BureauScore), BureauScore),
                         (nameof(VintageScore), VintageScore),
                         (nameof(SocialScore), SocialScore)
                     })
            {
                if (value < 0m || value > 100m)
                    yield return $"{name} must be between 0 and 100.";
            }
        }
    }
}
