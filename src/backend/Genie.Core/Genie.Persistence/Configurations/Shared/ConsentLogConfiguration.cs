using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;
using SharedKernel.Enums;

namespace Genie.Persistence.Configurations.Shared
{
    public sealed class ConsentLogConfiguration : IEntityTypeConfiguration<ConsentLog>
    {
        public void Configure(EntityTypeBuilder<ConsentLog> builder)
        {
            builder.ToTable("consent_logs", "shared");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.ConsentType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.ConsentedAt).IsRequired();
            builder.Property(x => x.ExpiryAt).IsRequired();
            builder.Property(x => x.RevokedAt);
            builder.Property(x => x.Purpose).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.GrantedToId).IsRequired();
            builder.Property(x => x.GrantedToType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);
            builder.Ignore(x => x.IsActive);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.ConsentType });
        }
    }
}
