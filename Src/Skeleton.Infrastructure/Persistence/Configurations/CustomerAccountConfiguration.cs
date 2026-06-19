using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

internal class CustomerAccountConfiguration : IEntityTypeConfiguration<CustomerAccount>
{
    public void Configure(EntityTypeBuilder<CustomerAccount> builder)
    {
        builder.ToTable("CustomerAccounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CustomerId)
               .IsRequired();

        builder.Property(a => a.Balance)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m);

        builder.Property(a => a.CreateBy)
               .HasMaxLength(100);

        builder.Property(a => a.ModifiedBy)
               .HasMaxLength(100);

        // One-to-one: Customer → CustomerAccount
        // Navigation defined on Customer side (Customer.Account)
        // Navigation defined on Account side (CustomerAccount.Customer)
        builder.HasOne(a => a.Customer)
               .WithOne(c => c.Account)
               .HasForeignKey<CustomerAccount>(a => a.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        // One-to-many: CustomerAccount → AccountTransactions
        builder.HasMany(a => a.Transactions)
               .WithOne()
               .HasForeignKey(t => t.AccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
