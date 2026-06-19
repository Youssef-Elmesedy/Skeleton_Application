using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.ProductName).IsRequired().HasMaxLength(500);
        builder.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(ci => ci.CreateBy).HasMaxLength(100);
        builder.Property(ci => ci.ModifiedBy).HasMaxLength(100);
    }
}
