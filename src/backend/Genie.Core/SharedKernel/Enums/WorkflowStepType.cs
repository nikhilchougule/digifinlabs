namespace SharedKernel.Enums
{
    /// <summary>The action a single WorkflowStep performs.</summary>
    public enum WorkflowStepType
    {
        Kyc,
        BankPull,
        GstPull,
        Score,
        Decision,
        ManualReview
    }
}
