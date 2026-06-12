using Los.Lending.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class LenderConfiguration : IEntityTypeConfiguration<Lender>
    {
        public void Configure(EntityTypeBuilder<Lender> builder)
        {
            builder.ToTable("lenders", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Name).HasMaxLength(300).IsRequired();
            builder.Property(x => x.LicenceNumber).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => x.LicenceNumber).IsUnique();
            builder.Property(x => x.LenderType).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.ApiEndpoint).HasMaxLength(500);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.HasMany(x => x.Products)
                .WithOne()
                .HasForeignKey(x => x.LenderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
