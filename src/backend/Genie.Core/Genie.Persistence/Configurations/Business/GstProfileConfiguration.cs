using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class GstProfileConfiguration : IEntityTypeConfiguration<GstProfile>
    {
        public void Configure(EntityTypeBuilder<GstProfile> builder)
        {
            builder.ToTable("gst_profiles", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.BusinessId).IsRequired();
            builder.HasIndex(x => x.BusinessId).IsUnique();
            builder.Property(x => x.Gstin).HasMaxLength(15).IsRequired();
            builder.HasIndex(x => x.Gstin).IsUnique();
            builder.Property(x => x.GstScore).HasPrecision(5, 2);
            builder.Property(x => x.LastSyncedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.Property(x => x.RawData)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasMany(x => x.Filings)
                .WithOne()
                .HasForeignKey(x => x.GstProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
