using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(500);
        builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.CreateBy).HasMaxLength(100);
        builder.Property(oi => oi.ModifiedBy).HasMaxLength(100);
    }
}
