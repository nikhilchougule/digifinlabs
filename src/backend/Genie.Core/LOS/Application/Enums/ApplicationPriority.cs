namespace Los.Application.Enums
{
    /// <summary>Processing priority — drives queue ordering and escalation.</summary>
    public enum ApplicationPriority
    {
        Normal,
        High,
        Escalated
    }
}
