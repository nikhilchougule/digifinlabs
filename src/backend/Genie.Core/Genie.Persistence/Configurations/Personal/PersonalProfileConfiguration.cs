using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal.Entities;
using Personal.Enums;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Personal
{
    public sealed class PersonalProfileConfiguration : IEntityTypeConfiguration<PersonalProfile>
    {
        public void Configure(EntityTypeBuilder<PersonalProfile> builder)
        {
            builder.ToTable("profiles", "personal");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId).IsUnique(); // 1:1 with User
            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DateOfBirth).IsRequired();
            builder.Property(x => x.Gender).HasColumnType("char(1)").IsRequired();
            builder.Property(x => x.EmploymentType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.EmployerName).HasMaxLength(200);
            builder.Property(x => x.Designation).HasMaxLength(200);
            builder.Property(x => x.EncryptionKeyVersion).HasDefaultValue(1);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // Nullable encrypted monthly income
            builder.OwnsOne(x => x.MonthlyIncome, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("monthly_income_cipher");
                enc.Property(e => e.KeyVersion).HasColumnName("monthly_income_key_ver").HasDefaultValue(1);
            });

            // Registered address as owned entity
            builder.OwnsOne(x => x.Address, addr =>
            {
                addr.Property(a => a.Line1).HasColumnName("address_line1").HasMaxLength(300);
                addr.Property(a => a.Line2).HasColumnName("address_line2").HasMaxLength(300);
                addr.Property(a => a.City).HasColumnName("address_city").HasMaxLength(100);
                addr.Property(a => a.State).HasColumnName("address_state").HasMaxLength(100);
                addr.Property(a => a.Pincode).HasColumnName("address_pincode").HasMaxLength(10);
                addr.Property(a => a.Country).HasColumnName("address_country").HasMaxLength(100).HasDefaultValue("India");
                addr.OwnsOne(a => a.Coordinates, geo =>
                {
                    geo.Property(g => g.Latitude).HasColumnName("address_lat");
                    geo.Property(g => g.Longitude).HasColumnName("address_lng");
                });
            });

            builder.Property(x => x.NomineeDetails)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
        }
    }
}
