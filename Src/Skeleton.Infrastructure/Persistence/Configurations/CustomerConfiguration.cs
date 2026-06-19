using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.PhoneNumber)
            .IsUnique();

        builder.Property(c => c.Email)
            .HasMaxLength(150);

        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.Property(c => c.Address)
            .HasMaxLength(300);

        builder.Property(c => c.Notes)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.CreateBy)
            .HasMaxLength(100);

        builder.Property(c => c.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(c => c.Account)
               .WithOne()
               .HasForeignKey<CustomerAccount>(a => a.CustomerId);
    }
}