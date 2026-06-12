using Los.Application.Entities;
using Los.Application.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class ApplicationDocumentConfiguration : IEntityTypeConfiguration<ApplicationDocument>
    {
        public void Configure(EntityTypeBuilder<ApplicationDocument> builder)
        {
            builder.ToTable("application_documents", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.DocType).HasConversion<string>().HasMaxLength(100).IsRequired();
            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.IsMandatory).HasDefaultValue(false);
            builder.Property(x => x.VaultId);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.UploadStage).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.RequestedById);
            builder.Property(x => x.UploadedById);
            builder.Property(x => x.VerifiedById);
            builder.Property(x => x.ExpiryDate);
            builder.Property(x => x.UploadedAt);
            builder.Property(x => x.VerifiedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.Property(x => x.AiExtractedData)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.Property(x => x.AnomalyFlags)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.ApplicationId);
            builder.HasIndex(x => new { x.ApplicationId, x.DocType });
        }
    }
}
