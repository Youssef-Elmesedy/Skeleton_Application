using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(d => d.Code).IsUnique();

        builder.Property(d => d.Description).IsRequired().HasMaxLength(500);
        builder.Property(d => d.Value).HasColumnType("decimal(18,2)");
        builder.Property(d => d.MinOrderAmount).HasColumnType("decimal(18,2)");
        builder.Property(d => d.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(d => d.Type).HasConversion<string>();
        builder.Property(d => d.CreateBy).HasMaxLength(100);
        builder.Property(d => d.ModifiedBy).HasMaxLength(100);
    }
}
