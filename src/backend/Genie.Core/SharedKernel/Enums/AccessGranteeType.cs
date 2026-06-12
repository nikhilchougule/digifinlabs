namespace SharedKernel.Enums
{
    /// <summary>
    /// Discriminator for the polymorphic AccessGrant.GrantedToId field.
    /// Identifies whether the grantee is an individual user, an API client (developer/NBFC),
    /// or a platform tenant (white-label partner).
    /// </summary>
    public enum AccessGranteeType
    {
        User,
        ApiClient,
        Tenant
    }
}
