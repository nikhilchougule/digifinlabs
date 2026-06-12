using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Enums;
using SharedKernel.Entities;

namespace Genie.Persistence.Configurations.Identity
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users", "identity");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.AuthProvider).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.KycStatus).HasConversion<string>().HasMaxLength(50).HasDefaultValue(KycStatus.Pending);
            builder.Property(x => x.KycRefId);
            builder.Property(x => x.ProfileRefId);
            builder.Property(x => x.TenantId).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.IsBlocked).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.HasIndex(x => x.TenantId);

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
