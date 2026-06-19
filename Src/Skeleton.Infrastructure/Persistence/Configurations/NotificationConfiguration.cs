using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).HasMaxLength(300).IsRequired();
        builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
        builder.Property(n => n.ActionUrl).HasMaxLength(500);
        builder.Property(n => n.IconClass).HasMaxLength(50);
        builder.Property(n => n.Metadata).HasMaxLength(2000);
        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(n => n.CreateBy).HasMaxLength(100);
        builder.Property(n => n.ModifiedBy).HasMaxLength(100);

        // Indexes for fast queries
        builder.HasIndex(n => new { n.UserId, n.IsRead });
        builder.HasIndex(n => n.CustomerId);
        builder.HasIndex(n => n.EmployeeId);
    }
}
