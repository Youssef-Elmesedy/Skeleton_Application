using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();

        builder.Property(e => e.Position).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Department).HasMaxLength(100);
        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.CreateBy).HasMaxLength(100);
        builder.Property(e => e.ModifiedBy).HasMaxLength(100);

        // Nav: AppUser references Employee (not the other way)
        // One-to-one navigated FROM AppUser side
    }
}
