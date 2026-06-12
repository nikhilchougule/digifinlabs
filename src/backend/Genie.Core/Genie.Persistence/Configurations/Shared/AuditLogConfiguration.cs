using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;
using SharedKernel.ValueObjects;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Shared
{
    public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs", "shared");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.EntityId).IsRequired();
            builder.Property(x => x.Action).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.PerformedBy).IsRequired();
            builder.Property(x => x.UserAgent).HasMaxLength(500);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.Property(x => x.IpAddress)
                .HasConversion(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new IpAddress(v))
                .HasMaxLength(45); // IPv6 max length

            builder.Property(x => x.Diff)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => new { x.EntityType, x.EntityId });
            builder.HasIndex(x => x.PerformedBy);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
