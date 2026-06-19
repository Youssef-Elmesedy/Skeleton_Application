using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class PaymentConfiguration : IEntityTypeConfiguration<Domain.Entities.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.GatewayFee).HasColumnType("decimal(18,2)");
        builder.Property(p => p.RefundAmount).HasColumnType("decimal(18,2)");
        builder.Property(p => p.MonthlyAmount).HasColumnType("decimal(18,2)");

        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Gateway).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.GatewayStatus).HasConversion<string>().HasMaxLength(50);

        builder.Property(p => p.GatewayTransactionId).HasMaxLength(500);
        builder.Property(p => p.GatewayReferenceCode).HasMaxLength(200);
        builder.Property(p => p.GatewayRawResponse).HasMaxLength(4000);
        builder.Property(p => p.WebhookPayload).HasMaxLength(8000);
        builder.Property(p => p.FailureReason).HasMaxLength(500);
        builder.Property(p => p.RefundReason).HasMaxLength(500);
        builder.Property(p => p.CreateBy).HasMaxLength(100);
        builder.Property(p => p.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(p => p.GatewayTransactionId);
        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.Status);

        builder.HasOne(p => p.Order)
               .WithOne(o => o.Payment)
               .HasForeignKey<Domain.Entities.Payment>(p => p.OrderId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
