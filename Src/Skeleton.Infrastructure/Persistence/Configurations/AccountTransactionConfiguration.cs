using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class AccountTransactionConfiguration : IEntityTypeConfiguration<AccountTransaction>
{
    public void Configure(EntityTypeBuilder<AccountTransaction> builder)
    {
        builder.ToTable("AccountTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.CustomerId)
               .IsRequired();

        // ✅ أضفنا AccountId
        builder.Property(t => t.AccountId)
               .IsRequired();

        builder.Property(t => t.Amount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(t => t.Type)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(t => t.Description)
               .HasMaxLength(500);

        builder.Property(t => t.Date)
               .HasDefaultValueSql("GETUTCDATE()")
               .IsRequired();

        builder.Property(c => c.CreateBy)
            .HasMaxLength(100);

        builder.Property(c => c.ModifiedBy)
            .HasMaxLength(100);

        // ✅ Index على AccountId + Date بدل CustomerId + Date
        builder.HasIndex(t => new { t.AccountId, t.Date });
    }
}