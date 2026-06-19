using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();

        builder.Property(u => u.Role)
               .HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(u => u.EmailVerificationToken).HasMaxLength(200);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(200);

        builder.Property(u => u.CustomerId).IsRequired(false);
        builder.Property(u => u.EmployeeId).IsRequired(false);
        builder.Property(u => u.CreateBy).HasMaxLength(100);
        builder.Property(u => u.ModifiedBy).HasMaxLength(100);
    }
}
