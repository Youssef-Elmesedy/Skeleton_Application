using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        // Aduit fileds
        builder.Property(x => x.CreateBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Uniq Index to ensure that category name is unique
        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
