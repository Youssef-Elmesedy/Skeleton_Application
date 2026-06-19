using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Body).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.CreateBy).HasMaxLength(100);
        builder.Property(r => r.ModifiedBy).HasMaxLength(100);

        // Unique: one review per customer per product
        builder.HasIndex(r => new { r.ProductId, r.CustomerId }).IsUnique();

        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
