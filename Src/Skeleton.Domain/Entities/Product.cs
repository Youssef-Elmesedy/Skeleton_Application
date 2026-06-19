using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;
public class Product : BaseEntity
{
    public string FullName { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal Discount { get; private set; }
    public decimal StockQuantity { get; private set; }
    public bool RequiresPrescription { get; private set; } = false;

    // Navigation property
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; } = null!;

    // Constructor for creating a new product
    public Product(
    string fullName,
    decimal price,
    string description,
    Guid? categoryId,
    decimal discount,
    decimal stockQuantity,
    bool requiresPrescription)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessException(ErrorType.Validation, "Product name is required.");

        if (price <= 0)
            throw new BusinessException(ErrorType.Validation, "Price must be greater than zero.");

        if (string.IsNullOrWhiteSpace(description))
            throw new BusinessException(ErrorType.Validation, "Description is required.");

        if (discount < 0)
            throw new BusinessException(ErrorType.Validation, "Discount cannot be negative.");

        if (discount > price)
            throw new BusinessException(ErrorType.Validation, "Discount cannot be greater than price.");

        if (stockQuantity < 0)
            throw new BusinessException(ErrorType.Validation, "Stock cannot be negative.");

        FullName = fullName;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        Discount = discount;
        StockQuantity = stockQuantity;
        RequiresPrescription = requiresPrescription;
    }

    // Method for updating product details
    public void Update(
    string? fullName = null,
    decimal? price = null,
    string? description = null,
    Guid? categoryId = null,
    decimal? discount = null,
    decimal? stockQuantity = null,
    bool? requiresPrescription = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName;

        if (price.HasValue)
        {
            if (price.Value <= 0)
                throw new BusinessException(ErrorType.Validation, "Price must be greater than zero.");

            if (Discount > price.Value)
                throw new BusinessException(ErrorType.Validation, "Discount cannot exceed price.");

            Price = price.Value;
        }

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        if (categoryId.HasValue)
            CategoryId = categoryId.Value;

        if (discount.HasValue)
        {
            if (discount.Value < 0)
                throw new BusinessException(ErrorType.Validation, "Discount cannot be negative.");

            if (discount.Value > Price)
                throw new BusinessException(ErrorType.Validation, "Discount cannot exceed price.");

            Discount = discount.Value;
        }

        if (stockQuantity.HasValue)
        {
            if (stockQuantity.Value < 0)
                throw new BusinessException(ErrorType.Validation, "Stock cannot be negative.");

            StockQuantity = stockQuantity.Value;
        }

        if (requiresPrescription.HasValue)
            RequiresPrescription = requiresPrescription.Value;
    }
    public void ChangeCategory(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public void RemoveCategory()
    {
        CategoryId = null;
    }
}
