using Los.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class ApplicationAssignmentConfiguration : IEntityTypeConfiguration<ApplicationAssignment>
    {
        public void Configure(EntityTypeBuilder<ApplicationAssignment> builder)
        {
            builder.ToTable("application_assignments", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.AssignedToId).IsRequired();
            builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.AssignedById);
            builder.Property(x => x.AssignedAt).IsRequired();
            builder.Property(x => x.AcceptedAt);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.HandoverNotes).HasMaxLength(2000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.HasIndex(x => x.ApplicationId);
            builder.HasIndex(x => x.AssignedToId);
            builder.HasIndex(x => new { x.ApplicationId, x.Role, x.IsActive });
        }
    }
}
