using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Discount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.StockQuantity)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.RequiresPrescription)
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // ✅ حذفنا IsRequired() من CreateBy لأنها nullable في BaseEntity
        builder.Property(p => p.CreateBy).HasMaxLength(100);
        builder.Property(p => p.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(p => p.FullName);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.StockQuantity);
        builder.HasIndex(p => new { p.CategoryId, p.Price });
    }
}