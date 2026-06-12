namespace SharedKernel.Enums
{
    /// <summary>Execution state of a single WorkflowStep.</summary>
    public enum WorkflowStepStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Skipped
    }
}
