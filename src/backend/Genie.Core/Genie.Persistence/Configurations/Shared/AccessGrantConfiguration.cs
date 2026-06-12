using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;

namespace Genie.Persistence.Configurations.Shared
{
    public sealed class AccessGrantConfiguration : IEntityTypeConfiguration<AccessGrant>
    {
        public void Configure(EntityTypeBuilder<AccessGrant> builder)
        {
            builder.ToTable("access_grants", "shared");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.VaultId).IsRequired();
            builder.Property(x => x.GrantedToId).IsRequired();
            builder.Property(x => x.GrantedToType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.ConsentId).IsRequired();
            builder.Property(x => x.GrantedAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.RevokedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);
            builder.Ignore(x => x.IsActive);

            builder.HasIndex(x => x.VaultId);
            builder.HasIndex(x => new { x.GrantedToId, x.GrantedToType });
            builder.HasIndex(x => x.ConsentId);
        }
    }
}
